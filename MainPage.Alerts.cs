namespace CampuGO;

public partial class MainPage
{
	private void BuildAlerts()
	{
		PageContent.Children.Add(ListCard(CampusData.Alerts.Select(alert => AlertRow(alert))));
	}
}
