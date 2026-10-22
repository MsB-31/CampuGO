namespace CampuGO.Services;

/// <summary>Services partagés de l'application (session, client de l'API, informations du campus).</summary>
public static class AppServices
{
	public static SessionStore Session { get; } = new(new MauiSecureStore());

	public static ApiClient Api { get; } = new(Session);

	public static CampusDataService Data { get; } = new(Api);
}
