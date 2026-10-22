using CampuGO.Services;
using Microsoft.Maui.Controls.Shapes;

namespace CampuGO;

public partial class MainPage
{
	private IReadOnlyList<(string Category, string Title, string Detail, string Icon)> _places = CampusData.Places;

	private void BuildExplore()
	{
		var entry = new Entry
		{
			Placeholder = "Rechercher un lieu ou un service",
			PlaceholderColor = Theme.TextMuted,
			TextColor = Theme.TextPrimary,
			FontFamily = Theme.Regular,
			FontSize = 15,
			BackgroundColor = Colors.Transparent,
			Text = _exploreQuery,
			ClearButtonVisibility = ClearButtonVisibility.WhileEditing,
			VerticalOptions = LayoutOptions.Center
		};

		var searchRow = new Grid
		{
			ColumnSpacing = 10,
			ColumnDefinitions = { new ColumnDefinition(GridLength.Auto), new ColumnDefinition(GridLength.Star) },
			Children = { IconView(Icons.Search, Theme.TextMuted, 20), entry }
		};
		Grid.SetColumn(entry, 1);

		var searchBox = new Border
		{
			BackgroundColor = Theme.Surface,
			Stroke = Theme.Border,
			StrokeThickness = 1,
			StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(Theme.RadiusControl) },
			Padding = new Thickness(12, 0),
			HeightRequest = 48,
			Content = searchRow
		};

		var results = new VerticalStackLayout { Spacing = 0 };
		var chips = new List<(Button Button, string Filter)>();

		void Refresh()
		{
			foreach (var (button, filter) in chips)
			{
				var selected = filter == _exploreFilter;
				button.BackgroundColor = selected ? Theme.Accent : Theme.Surface;
				button.TextColor = selected ? Theme.OnAccent : Theme.TextSecondary;
				button.BorderColor = selected ? Theme.Accent : Theme.Border;
			}

			results.Children.Clear();
			var matches = _places.Where(place =>
				(_exploreFilter == "Tout" || place.Category == _exploreFilter) &&
				(string.IsNullOrWhiteSpace(_exploreQuery) ||
					place.Title.Contains(_exploreQuery, StringComparison.OrdinalIgnoreCase) ||
					place.Detail.Contains(_exploreQuery, StringComparison.OrdinalIgnoreCase))).ToList();

			if (matches.Count == 0)
			{
				results.Children.Add(Text("Aucun résultat pour cette recherche.", 14, Theme.TextSecondary));
				return;
			}

			results.Children.Add(ListCard(matches.Select(place => PlaceRow(place))));
		}

		var chipRow = new HorizontalStackLayout { Spacing = 8 };
		foreach (var filter in CampusData.ExploreFilters)
		{
			var value = filter;
			var chip = new Button
			{
				Text = filter,
				FontSize = 13,
				FontFamily = Theme.SemiBold,
				CornerRadius = 18,
				HeightRequest = 36,
				Padding = new Thickness(16, 0),
				BorderWidth = 1
			};
			chip.Clicked += (_, _) => { _exploreFilter = value; Refresh(); };
			chips.Add((chip, filter));
			chipRow.Children.Add(chip);
		}

		entry.TextChanged += (_, args) => { _exploreQuery = args.NewTextValue ?? string.Empty; Refresh(); };

		PageContent.Children.Add(new VerticalStackLayout
		{
			Spacing = 12,
			Children =
			{
				searchBox,
				new ScrollView
				{
					Orientation = ScrollOrientation.Horizontal,
					HorizontalScrollBarVisibility = ScrollBarVisibility.Never,
					Content = chipRow
				}
			}
		});
		PageContent.Children.Add(results);
		Refresh();

		// Les lieux affichés d'abord sont les données locales ; ceux de l'API les remplacent dès leur arrivée.
		RunInBackground(async () =>
		{
			var remote = await AppServices.Data.TryGetPlacesAsync();
			if (remote is null || _current != Screen.Explore) return;
			_places = remote;
			Refresh();
		});
	}
}
