namespace CampuGO;

public partial class MainPage
{
	private void BuildAlertsPage()
	{
		PageContent.Children.Add(CreateHeader());
		PageContent.Children.Add(PageTitle("Les alertes", "Les infos utiles, au bon moment."));
		foreach (var alert in CampusData.Alerts)
			PageContent.Children.Add(AlertCard(alert.Category, alert.Title, alert.Message, alert.Time, alert.Urgent));
		PageContent.Children.Add(Text("Tout est à jour. Les nouvelles alertes apparaîtront ici.", 12, SecondaryText));
	}

	private View AlertCard(string category, string title, string message, string time, bool urgent)
	{
		var accent = urgent ? AlertColor : Teal;
		return Frame(new VerticalStackLayout
		{
			Spacing = 10,
			Children =
			{
				Text($"●  {category}     {time}", 10, accent, true),
				Text(title, 16, PrimaryText, true), Text(message, 13, SecondaryText)
			}
		}, 15);
	}
}
