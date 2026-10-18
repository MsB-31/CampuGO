namespace CampuGO;

public partial class MainPage
{
	private void BuildAgendaPage()
	{
		PageContent.Children.Add(CreateHeader());
		PageContent.Children.Add(PageTitle("Ton agenda", "Les rendez-vous de la vie étudiante."));
		var dates = new HorizontalStackLayout { Spacing = 9 };
		foreach (var day in new[] { ("MAR", "20"), ("MER", "21"), ("JEU", "22"), ("VEN", "23"), ("SAM", "24") })
		{
			var selected = day.Item2 == "22";
			dates.Children.Add(Frame(new VerticalStackLayout
			{
				Spacing = 5, HorizontalOptions = LayoutOptions.Center,
				Children = { Text(day.Item1, 10, selected ? OnTeal : SecondaryText, true), Text(day.Item2, 17, selected ? OnTeal : PrimaryText, true) }
			}, 10, selected ? Teal : Surface));
		}
		PageContent.Children.Add(new ScrollView { Orientation = ScrollOrientation.Horizontal, Content = dates });
		PageContent.Children.Add(Section("Jeudi 22 octobre", "Cette semaine", () => { }));
		foreach (var item in CampusData.Schedule)
			PageContent.Children.Add(ScheduleItem(item.Time, item.Title, item.Place, item.Category));
		PageContent.Children.Add(Text("PENSE-BÊTE", 11, SecondaryText, true));
		PageContent.Children.Add(InfoCard("✦", "Garde tes rendez-vous sous la main", "Retrouve ici les événements de l’Université de Douala et de l’ENSPD."));
	}

	private View ScheduleItem(string time, string title, string place, string category)
	{
		var timeLabel = Text(time, 13, Teal, true);
		timeLabel.WidthRequest = 48;
		var details = new VerticalStackLayout
		{
			Spacing = 5,
			Children = { Text(title, 15, PrimaryText, true), Text(place, 12, SecondaryText), Text(category, 10, Teal, true) }
		};
		return Frame(new HorizontalStackLayout { Spacing = 12, Children = { timeLabel, details } }, 14);
	}
}
