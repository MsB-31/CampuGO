namespace CampuGO;

public partial class MainPage
{
	private void BuildExplorePage()
	{
		PageContent.Children.Add(CreateHeader());
		PageContent.Children.Add(PageTitle("Explorer", "Tout ce qui fait vivre ton campus."));
		var search = new Entry
		{
			Placeholder = "Un lieu, une activité, un service…", PlaceholderColor = MutedText,
			TextColor = PrimaryText, BackgroundColor = Surface, FontSize = 15, HeightRequest = 52,
			ClearButtonVisibility = ClearButtonVisibility.WhileEditing
		};
		search.TextChanged += (_, args) => { _searchQuery = args.NewTextValue ?? string.Empty; UpdateExploreResults(); };
		PageContent.Children.Add(Frame(search, 12));

		var filters = new HorizontalStackLayout { Spacing = 8 };
		foreach (var filter in CampusData.ExploreFilters)
		{
			filters.Children.Add(new Button
			{
				Text = filter, FontSize = 12, FontFamily = "OpenSansSemibold",
				TextColor = filter == _selectedFilter ? OnTeal : SecondaryText,
				BackgroundColor = filter == _selectedFilter ? Teal : Surface,
				CornerRadius = 16, Padding = new Thickness(13, 8), MinimumHeightRequest = 38,
				Command = new Command(() => { _selectedFilter = filter; ShowTab("Explorer"); })
			});
		}
		PageContent.Children.Add(new ScrollView { Orientation = ScrollOrientation.Horizontal, Content = filters });
		PageContent.Children.Add(Text("À DÉCOUVRIR", 11, SecondaryText, true));
		var results = new VerticalStackLayout { Spacing = 12, AutomationId = "ExploreResults" };
		PageContent.Children.Add(results);
		FillExploreResults(results);
	}

	private void UpdateExploreResults()
	{
		var results = PageContent.Children.OfType<VerticalStackLayout>().FirstOrDefault(view => view.AutomationId == "ExploreResults");
		if (results is not null) FillExploreResults(results);
	}

	private void FillExploreResults(VerticalStackLayout results)
	{
		results.Children.Clear();
		var matches = CampusData.Places.Where(place =>
			(_selectedFilter == "Tout" || place.Category == _selectedFilter) &&
			(string.IsNullOrWhiteSpace(_searchQuery) ||
				place.Title.Contains(_searchQuery, StringComparison.OrdinalIgnoreCase) ||
				place.Detail.Contains(_searchQuery, StringComparison.OrdinalIgnoreCase))).ToArray();
		if (matches.Length == 0)
		{
			results.Children.Add(Text("Aucun résultat. Essaie un autre mot-clé.", 14, SecondaryText));
			return;
		}
		foreach (var place in matches)
			results.Children.Add(InfoCard(place.Icon, place.Title, $"{place.Detail}\n{place.Category.ToUpperInvariant()}"));
	}
}
