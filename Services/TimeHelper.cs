using System.Globalization;

namespace CampuGO.Services;

public static class TimeHelper
{
	/// <summary>Les dates lues dans la base ne portent pas de fuseau : on les interprète comme des dates UTC.</summary>
	public static DateTime ToUtc(DateTime value) =>
		value.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(value, DateTimeKind.Utc) : value.ToUniversalTime();

	public static string RelativeTime(DateTime utc, DateTime? now = null)
	{
		var moment = ToUtc(utc);
		var span = (now ?? DateTime.UtcNow) - moment;
		if (span < TimeSpan.Zero) span = TimeSpan.Zero;

		if (span.TotalMinutes < 1) return "À l’instant";
		if (span.TotalMinutes < 60) return $"Il y a {(int)span.TotalMinutes} min";
		if (span.TotalHours < 24) return $"Il y a {(int)span.TotalHours} h";
		if (span.TotalHours < 48) return "Hier";
		return moment.ToLocalTime().ToString("d MMM", new CultureInfo("fr-FR"));
	}
}
