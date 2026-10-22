using System.Globalization;

namespace CampuGO.Services;

/// <summary>Jetons de la session : en mémoire pendant l'exécution, et dans le stockage sécurisé entre deux lancements.</summary>
public sealed class SessionStore(ISecureStore store)
{
	private const string AccessKey = "campugo.access_token";
	private const string RefreshKey = "campugo.refresh_token";
	private const string ExpiresKey = "campugo.access_expires";

	public string? AccessToken { get; private set; }

	public string? RefreshToken { get; private set; }

	public DateTime AccessTokenExpiresAt { get; private set; }

	/// <summary>Vrai tant qu'un jeton de renouvellement est disponible.</summary>
	public bool IsAuthenticated => !string.IsNullOrEmpty(RefreshToken);

	public async Task LoadAsync()
	{
		AccessToken = await store.GetAsync(AccessKey);
		RefreshToken = await store.GetAsync(RefreshKey);
		var expires = await store.GetAsync(ExpiresKey);
		AccessTokenExpiresAt = DateTime.TryParse(expires, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var parsed)
			? TimeHelper.ToUtc(parsed)
			: DateTime.MinValue;
	}

	public async Task SaveAsync(AuthSession session)
	{
		AccessToken = session.AccessToken;
		RefreshToken = session.RefreshToken;
		AccessTokenExpiresAt = TimeHelper.ToUtc(session.AccessTokenExpiresAt);

		await store.SetAsync(AccessKey, AccessToken);
		await store.SetAsync(RefreshKey, RefreshToken);
		await store.SetAsync(ExpiresKey, AccessTokenExpiresAt.ToString("O", CultureInfo.InvariantCulture));
	}

	public void Clear()
	{
		AccessToken = null;
		RefreshToken = null;
		AccessTokenExpiresAt = DateTime.MinValue;

		store.Remove(AccessKey);
		store.Remove(RefreshKey);
		store.Remove(ExpiresKey);
	}
}
