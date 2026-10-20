namespace CampuGO;

/// <summary>Jetons de design (couleurs, polices, rayons) alignés sur la maquette Figma.</summary>
public static class Theme
{
	public static readonly Color Background = Color.FromArgb("#101415");
	public static readonly Color Surface = Color.FromArgb("#181C1D");
	public static readonly Color SurfaceRaised = Color.FromArgb("#272B2C");
	public static readonly Color Border = Color.FromArgb("#2E3334");

	public static readonly Color TextPrimary = Color.FromArgb("#E0E3E4");
	public static readonly Color TextSecondary = Color.FromArgb("#BACAC5");
	public static readonly Color TextMuted = Color.FromArgb("#859490");

	public static readonly Color Accent = Color.FromArgb("#4EDEA3");
	public static readonly Color OnAccent = Color.FromArgb("#003731");
	public static readonly Color Danger = Color.FromArgb("#FFB4AB");
	public static readonly Color Warning = Color.FromArgb("#E9C46A");

	public const string Regular = "InterRegular";
	public const string SemiBold = "InterSemiBold";

	public const double RadiusCard = 12;
	public const double RadiusControl = 8;
}
