namespace CampuGO;

/// <summary>
/// Profil étudiant enregistré localement sur l'appareil (Preferences MAUI).
/// Le mot de passe n'est volontairement pas stocké : il n'y a pas encore de serveur d'authentification.
/// </summary>
public sealed class UserProfile
{
	private const string Prefix = "profile.";
	private static readonly string[] Keys = { "fullName", "email", "phone", "institution", "track", "level", "campus" };

	public string FullName { get; set; } = string.Empty;
	public string Email { get; set; } = string.Empty;
	public string Phone { get; set; } = string.Empty;
	public string Institution { get; set; } = string.Empty;
	public string Track { get; set; } = string.Empty;
	public string Level { get; set; } = string.Empty;
	public string Campus { get; set; } = string.Empty;

	public string Initial => string.IsNullOrWhiteSpace(FullName) ? "?" : FullName.Trim()[..1].ToUpperInvariant();

	public static UserProfile? Load()
	{
		var prefs = Preferences.Default;
		if (!prefs.ContainsKey(Prefix + "email")) return null;
		return new UserProfile
		{
			FullName = prefs.Get(Prefix + "fullName", string.Empty),
			Email = prefs.Get(Prefix + "email", string.Empty),
			Phone = prefs.Get(Prefix + "phone", string.Empty),
			Institution = prefs.Get(Prefix + "institution", string.Empty),
			Track = prefs.Get(Prefix + "track", string.Empty),
			Level = prefs.Get(Prefix + "level", string.Empty),
			Campus = prefs.Get(Prefix + "campus", string.Empty)
		};
	}

	public void Save()
	{
		var prefs = Preferences.Default;
		prefs.Set(Prefix + "fullName", FullName);
		prefs.Set(Prefix + "email", Email);
		prefs.Set(Prefix + "phone", Phone);
		prefs.Set(Prefix + "institution", Institution);
		prefs.Set(Prefix + "track", Track);
		prefs.Set(Prefix + "level", Level);
		prefs.Set(Prefix + "campus", Campus);
	}

	public static void Clear()
	{
		foreach (var key in Keys) Preferences.Default.Remove(Prefix + key);
	}
}
