namespace CampuGO.Services;

/// <summary>
/// Informations du campus lues sur l'API. Chaque méthode renvoie null si le serveur est indisponible :
/// l'application garde alors ses données locales.
/// </summary>
public sealed class CampusDataService(ApiClient api)
{
	public async Task<List<(string Category, string Title, string Detail, string Icon)>?> TryGetPlacesAsync(CancellationToken ct = default)
	{
		var result = await api.GetPlacesAsync(ct);
		if (!result.IsSuccess || result.Value is not { } items) return null;
		return items.Select(p => (p.Category, p.Title, p.Detail, p.Icon)).ToList();
	}

	public async Task<List<CampusEvent>?> TryGetEventsAsync(DateTime date, CancellationToken ct = default)
	{
		var result = await api.GetEventsAsync(DateOnly.FromDateTime(date), ct);
		if (!result.IsSuccess || result.Value is not { } items) return null;
		return items.Select(e => new CampusEvent(date.DayOfWeek, e.Time, e.Title, e.Location, e.Category)).ToList();
	}

	public async Task<List<(string Category, string Title, string Message, string Time, bool Urgent)>?> TryGetAlertsAsync(CancellationToken ct = default)
	{
		var result = await api.GetAlertsAsync(ct);
		if (!result.IsSuccess || result.Value is not { } items) return null;
		return items.Select(a => (a.Category, a.Title, a.Message, TimeHelper.RelativeTime(a.CreatedAt), a.Urgent)).ToList();
	}
}
