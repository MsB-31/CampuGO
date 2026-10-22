namespace CampuGO.Services;

// Objets échangés avec l'API CampuGO (JSON en camelCase).

public sealed record ApiUser(Guid Id, string Email, string FullName, string? Phone, string Institution, string Track, string Level, string Campus);

public sealed record AuthSession(string AccessToken, DateTime AccessTokenExpiresAt, string RefreshToken, DateTime RefreshTokenExpiresAt, ApiUser User);

public sealed record ApiPlace(string Category, string Title, string Detail, string Icon);

public sealed record ApiEvent(string Time, string Title, string Location, string Category);

public sealed record ApiAlert(string Category, string Title, string Message, DateTime CreatedAt, bool Urgent);

public sealed record RegisterPayload(string FullName, string Email, string? Phone, string Institution, string Track, string Level, string Campus, string Password);

public sealed record LoginPayload(string Email, string Password);

public sealed record RefreshPayload(string RefreshToken);

/// <summary>Résultat d'un appel à l'API. Un statut 0 signifie que le serveur n'a pas pu être joint.</summary>
public sealed record ApiResult<T>(bool IsSuccess, T? Value, int Status, string? Message)
{
	public bool IsNetworkError => Status == 0;

	public static ApiResult<T> Success(T value, int status = 200) => new(true, value, status, null);

	public static ApiResult<T> Failure(int status, string? message) => new(false, default, status, message);
}
