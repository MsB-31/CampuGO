namespace CampuGO;

public partial class MainPage
{
	private void BuildHomePage()
	{
		PageContent.Children.Add(CreateHeader());
		PageContent.Children.Add(new VerticalStackLayout
		{
			Spacing = 5,
			Children =
			{
				Text("Bienvenue sur CampuGO", 25, PrimaryText, true),
				Text("Ton campus de Douala t’attend.", 14, SecondaryText)
			}
		});
		PageContent.Children.Add(CreateHero());
		PageContent.Children.Add(Section("Pour toi", "Tout voir", () => ShowTab("Explorer")));
		PageContent.Children.Add(CreateShortcuts());
		PageContent.Children.Add(Section("À ne pas manquer", "L’agenda", () => ShowTab("Agenda")));
		var next = CampusData.Schedule[0];
		PageContent.Children.Add(EventCard("JEUDI 22 OCT.", next.Title, $"{next.Time}  ·  {next.Place}", "Découvrir", () => ShowTab("Agenda")));
		PageContent.Children.Add(InfoCard("✦", "La cafétéria du campus", "Repas et boissons à midi, au campus de Logbessou comme à Bassa."));
	}

	private View CreateHero()
	{
		return Frame(new VerticalStackLayout
		{
			Spacing = 14,
			Children =
			{
				Text("●  LA VIE DU CAMPUS", 11, Teal, true),
				Text("Ton campus,\ntout près.", 29, PrimaryText, true),
				Text("Les lieux, les gens et les bons plans\nde l’Université de Douala et de l’ENSPD.", 14, SecondaryText),
				ActionButton("Explorer le campus", () => ShowTab("Explorer"), true)
			}
		}, 20, Color.FromArgb("#203031"));
	}

	private View CreateShortcuts()
	{
		var grid = new Grid
		{
			ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Star) },
			ColumnSpacing = 10
		};
		var shortcuts = new[] { ("⌕", "Explorer"), ("▦", "Agenda"), ("♧", "Alertes") };
		for (var index = 0; index < shortcuts.Length; index++)
		{
			var shortcut = shortcuts[index];
			var tile = new VerticalStackLayout
			{
				Spacing = 8, Padding = new Thickness(12, 14), BackgroundColor = Surface,
				Children = { Text(shortcut.Item1, 22, Teal, true), Text(shortcut.Item2, 12, PrimaryText, true) }
			};
			var tap = new TapGestureRecognizer();
			tap.Tapped += (_, _) => ShowTab(shortcut.Item2);
			tile.GestureRecognizers.Add(tap);
			Grid.SetColumn(tile, index);
			grid.Children.Add(tile);
		}
		return grid;
	}
}
