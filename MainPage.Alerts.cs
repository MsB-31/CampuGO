using CampuGO.Services;

namespace CampuGO;

public partial class MainPage
{
	private IReadOnlyList<(string Category, string Title, string Message, string Time, bool Urgent)> _alerts = CampusData.Alerts;

	private void BuildAlerts()
	{
		var host = new VerticalStackLayout();

		void Render()
		{
			host.Children.Clear();
			host.Children.Add(ListCard(_alerts.Select(alert => AlertRow(alert))));
		}

		Render();
		PageContent.Children.Add(host);

		// Alertes locales affichées d'abord ; celles de l'API les remplacent dès leur arrivée.
		RunInBackground(async () =>
		{
			var remote = await AppServices.Data.TryGetAlertsAsync();
			if (remote is null || _current != Screen.Alerts) return;
			_alerts = remote;
			Render();
		});
	}
}
