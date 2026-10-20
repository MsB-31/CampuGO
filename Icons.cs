namespace CampuGO;

/// <summary>Icônes de la police Material Icons (Apache 2.0), utilisées comme sources d'image vectorielles.</summary>
public static class Icons
{
	public const string FontName = "MaterialIcons";

	public const string Home = "\ue88a";
	public const string Search = "\ue8b6";
	public const string Calendar = "\ue935";
	public const string Bell = "\ue7f4";
	public const string Person = "\ue7fd";
	public const string Back = "\ue5c4";
	public const string Visibility = "\ue8f4";
	public const string VisibilityOff = "\ue8f5";
	public const string Place = "\ue55f";
	public const string Restaurant = "\ue56c";
	public const string Library = "\ue54b";
	public const string School = "\ue80c";
	public const string Groups = "\uf233";
	public const string Soccer = "\uea2f";

	/// <summary>Associe le nom d'icône stocké dans les données au caractère de la police.</summary>
	public static string FromName(string name) => name switch
	{
		"restaurant" => Restaurant,
		"library" => Library,
		"school" => School,
		"groups" => Groups,
		"soccer" => Soccer,
		_ => Place
	};

	public static ImageSource Source(string glyph, Color color, double size = 24) => new FontImageSource
	{
		FontFamily = FontName,
		Glyph = glyph,
		Color = color,
		Size = size
	};
}
