using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace CampuGO.Services;

/// <summary>
/// Client de l'API CampuGO : HTTPS, jeton d'accès ajouté automatiquement, renouvellement de la session
/// à l'expiration ou après un refus (401), et résultats typés sans exception réseau.
/// </summary>
public sealed class ApiClient
{
	/// <summary>
	/// Adresse de l'API. En développement, « adb reverse tcp:7080 tcp:7080 » relie ce port du téléphone
	/// (ou de l'émulateur) à celui de l'ordinateur. À remplacer par l'adresse HTTPS réelle d'un serveur déployé.
	/// </summary>
	public const string DefaultBaseUrl = "https://localhost:7080";

	private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

	private readonly HttpClient _http;
	private readonly SessionStore _session;
	private readonly SemaphoreSlim _refreshLock = new(1, 1);

	public ApiClient(SessionStore session, HttpMessageHandler? handler = null, string baseUrl = DefaultBaseUrl)
	{
		_session = session;
		_http = new HttpClient(handler ?? CreateHandler())
		{
			BaseAddress = new Uri(baseUrl),
			Timeout = TimeSpan.FromSeconds(8)
		};
	}

	private sealed record Raw(int Status, string? Body, string? Error);

	// ---------------------------------------------------------------- Compte

	public Task<ApiResult<AuthSession>> RegisterAsync(RegisterPayload payload, CancellationToken ct = default) =>
		SendAuthAsync("api/auth/register", payload, ct);

	public Task<ApiResult<AuthSession>> LoginAsync(LoginPayload payload, CancellationToken ct = default) =>
		SendAuthAsync("api/auth/login", payload, ct);

	/// <summary>Révoque la session côté serveur (au mieux) puis efface les jetons de l'appareil.</summary>
	public async Task<ApiResult<bool>> LogoutAsync(CancellationToken ct = default)
	{
		var refresh = _session.RefreshToken;
		var result = ApiResult<bool>.Success(true);
		if (!string.IsNullOrEmpty(refresh))
		{
			var raw = await SendRawAsync(HttpMethod.Post, "api/auth/logout", new RefreshPayload(refresh), true, ct).ConfigureAwait(false);
			result = ParseNoContent(raw);
		}

		_session.Clear();
		return result;
	}

	public async Task<ApiResult<ApiUser>> GetMeAsync(CancellationToken ct = default) =>
		Parse<ApiUser>(await SendRawAsync(HttpMethod.Get, "api/me", null, true, ct).ConfigureAwait(false));

	/// <summary>Supprime le compte sur le serveur ; les jetons ne sont effacés qu'en cas de succès.</summary>
	public async Task<ApiResult<bool>> DeleteAccountAsync(CancellationToken ct = default)
	{
		var result = ParseNoContent(await SendRawAsync(HttpMethod.Delete, "api/me", null, true, ct).ConfigureAwait(false));
		if (result.IsSuccess) _session.Clear();
		return result;
	}

	// ---------------------------------------------------------------- Informations du campus

	public async Task<ApiResult<List<ApiPlace>>> GetPlacesAsync(CancellationToken ct = default) =>
		Parse<List<ApiPlace>>(await SendRawAsync(HttpMethod.Get, "api/places", null, false, ct).ConfigureAwait(false));

	public async Task<ApiResult<List<ApiEvent>>> GetEventsAsync(DateOnly date, CancellationToken ct = default)
	{
		var path = "api/events?date=" + date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
		return Parse<List<ApiEvent>>(await SendRawAsync(HttpMethod.Get, path, null, false, ct).ConfigureAwait(false));
	}

	public async Task<ApiResult<List<ApiAlert>>> GetAlertsAsync(CancellationToken ct = default) =>
		Parse<List<ApiAlert>>(await SendRawAsync(HttpMethod.Get, "api/alerts", null, false, ct).ConfigureAwait(false));

	// ---------------------------------------------------------------- Mécanique

	private async Task<ApiResult<AuthSession>> SendAuthAsync(string path, object body, CancellationToken ct)
	{
		var result = Parse<AuthSession>(await SendRawAsync(HttpMethod.Post, path, body, false, ct).ConfigureAwait(false));
		if (result.IsSuccess && result.Value is not null)
			await _session.SaveAsync(result.Value).ConfigureAwait(false);
		return result;
	}

	/// <summary>Envoie une requête ; si elle est authentifiée, renouvelle la session au besoin et rejoue la requête une fois.</summary>
	private async Task<Raw> SendRawAsync(HttpMethod method, string path, object? body, bool authenticated, CancellationToken ct)
	{
		if (!authenticated)
			return await SendCoreAsync(method, path, body, null, ct).ConfigureAwait(false);

		for (var attempt = 0; attempt < 2; attempt++)
		{
			if (!await EnsureAccessTokenAsync(ct).ConfigureAwait(false))
				return _session.IsAuthenticated ? Unreachable() : new Raw(401, null, "Session expirée.");

			var token = _session.AccessToken;
			var raw = await SendCoreAsync(method, path, body, token, ct).ConfigureAwait(false);
			if (raw.Status != 401 || attempt == 1) return raw;

			if (!await RefreshAsync(token, ct).ConfigureAwait(false))
				return _session.IsAuthenticated ? Unreachable() : raw;
		}

		return new Raw(401, null, "Session expirée.");
	}

	private async Task<Raw> SendCoreAsync(HttpMethod method, string path, object? body, string? bearer, CancellationToken ct)
	{
		using var request = new HttpRequestMessage(method, path);
		if (body is not null) request.Content = JsonContent.Create(body, body.GetType(), options: Json);
		if (bearer is not null) request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearer);

		try
		{
			using var response = await _http.SendAsync(request, ct).ConfigureAwait(false);
			var text = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
			return new Raw((int)response.StatusCode, text, null);
		}
		catch (Exception ex) when (ex is HttpRequestException or IOException or System.Security.Authentication.AuthenticationException)
		{
			return Unreachable();
		}
		catch (OperationCanceledException) when (!ct.IsCancellationRequested)
		{
			return new Raw(0, null, "Le serveur ne répond pas.");
		}
	}

	private static Raw Unreachable() => new(0, null, "Serveur injoignable.");

	private async Task<bool> EnsureAccessTokenAsync(CancellationToken ct)
	{
		if (!string.IsNullOrEmpty(_session.AccessToken) && _session.AccessTokenExpiresAt > DateTime.UtcNow.AddSeconds(30))
			return true;

		return await RefreshAsync(_session.AccessToken, ct).ConfigureAwait(false);
	}

	/// <summary>
	/// Échange le jeton de renouvellement contre une nouvelle paire de jetons. Un seul renouvellement à la fois :
	/// les appels simultanés réutilisent le résultat du premier. Un échec réseau conserve la session.
	/// </summary>
	private async Task<bool> RefreshAsync(string? staleAccessToken, CancellationToken ct)
	{
		await _refreshLock.WaitAsync(ct).ConfigureAwait(false);
		try
		{
			if (!string.IsNullOrEmpty(_session.AccessToken)
				&& _session.AccessToken != staleAccessToken
				&& _session.AccessTokenExpiresAt > DateTime.UtcNow.AddSeconds(30))
				return true;

			var refresh = _session.RefreshToken;
			if (string.IsNullOrEmpty(refresh)) return false;

			var raw = await SendCoreAsync(HttpMethod.Post, "api/auth/refresh", new RefreshPayload(refresh), null, ct).ConfigureAwait(false);
			if (raw.Status == 0) return false;

			var result = Parse<AuthSession>(raw);
			if (result.IsSuccess && result.Value is not null)
			{
				await _session.SaveAsync(result.Value).ConfigureAwait(false);
				return true;
			}

			if (raw.Status is 400 or 401) _session.Clear();
			return false;
		}
		finally
		{
			_refreshLock.Release();
		}
	}

	private static ApiResult<T> Parse<T>(Raw raw)
	{
		if (raw.Status == 0) return ApiResult<T>.Failure(0, raw.Error);

		if (raw.Status is >= 200 and < 300)
		{
			try
			{
				var value = JsonSerializer.Deserialize<T>(raw.Body ?? string.Empty, Json);
				if (value is not null) return ApiResult<T>.Success(value, raw.Status);
			}
			catch (JsonException) { /* corps inattendu : traité ci-dessous */ }

			return ApiResult<T>.Failure(raw.Status, "Réponse inattendue du serveur.");
		}

		return ApiResult<T>.Failure(raw.Status, ErrorMessage(raw));
	}

	private static ApiResult<bool> ParseNoContent(Raw raw)
	{
		if (raw.Status == 0) return ApiResult<bool>.Failure(0, raw.Error);
		return raw.Status is >= 200 and < 300
			? ApiResult<bool>.Success(true, raw.Status)
			: ApiResult<bool>.Failure(raw.Status, ErrorMessage(raw));
	}

	/// <summary>Message en français : titre renvoyé par l'API (sauf erreurs de validation, en anglais), sinon texte par défaut.</summary>
	private static string ErrorMessage(Raw raw)
	{
		if (raw.Status != 400 && !string.IsNullOrWhiteSpace(raw.Body))
		{
			try
			{
				using var doc = JsonDocument.Parse(raw.Body);
				if (doc.RootElement.ValueKind == JsonValueKind.Object
					&& doc.RootElement.TryGetProperty("title", out var title)
					&& title.ValueKind == JsonValueKind.String
					&& !string.IsNullOrWhiteSpace(title.GetString()))
					return title.GetString()!;
			}
			catch (JsonException) { /* corps non JSON : message par défaut */ }
		}

		return raw.Status switch
		{
			400 => "Informations invalides. Vérifiez les champs saisis.",
			401 => "Identifiants invalides.",
			409 => "Un compte existe déjà avec cette adresse e-mail.",
			429 => "Trop de tentatives. Réessayez plus tard.",
			>= 500 => "Erreur du serveur. Réessayez plus tard.",
			_ => "Une erreur est survenue."
		};
	}

	private static HttpMessageHandler CreateHandler()
	{
#if ANDROID && DEBUG
		// Développement uniquement : accepte le certificat auto-signé de l'API locale (localhost).
		// Cette branche n'existe pas dans les versions Release, qui valident les certificats normalement.
		return new Xamarin.Android.Net.AndroidMessageHandler
		{
			ServerCertificateCustomValidationCallback = (request, certificate, chain, errors) =>
				errors == System.Net.Security.SslPolicyErrors.None
				|| (request.RequestUri?.Host == "localhost" && certificate?.Issuer == "CN=localhost")
		};
#else
		return new HttpClientHandler();
#endif
	}
}
