using Microsoft.Maui.Controls.Shapes;

namespace CampuGO;

/// <summary>Composants visuels réutilisables par tous les écrans.</summary>
public partial class MainPage
{
	private static Label Text(string text, double size, Color color, bool semibold = false, TextAlignment alignment = TextAlignment.Start) => new()
	{
		Text = text,
		FontSize = size,
		TextColor = color,
		FontFamily = semibold ? Theme.SemiBold : Theme.Regular,
		LineBreakMode = LineBreakMode.WordWrap,
		HorizontalTextAlignment = alignment
	};

	private static Image IconView(string glyph, Color color, double size = 22) => new()
	{
		Source = Icons.Source(glyph, color, size),
		WidthRequest = size,
		HeightRequest = size,
		HorizontalOptions = LayoutOptions.Center,
		VerticalOptions = LayoutOptions.Center
	};

	private static Border Card(View content, Thickness? padding = null) => new()
	{
		Padding = padding ?? new Thickness(16),
		BackgroundColor = Theme.Surface,
		Stroke = Theme.Border,
		StrokeThickness = 1,
		StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(Theme.RadiusCard) },
		Content = content
	};

	private static BoxView Divider() => new()
	{
		Color = Theme.Border,
		HeightRequest = 1,
		Margin = new Thickness(16, 0)
	};

	/// <summary>Une seule carte contenant plusieurs lignes séparées par un filet.</summary>
	private static Border ListCard(IEnumerable<View> rows)
	{
		var stack = new VerticalStackLayout { Spacing = 0 };
		var first = true;
		foreach (var row in rows)
		{
			if (!first) stack.Children.Add(Divider());
			stack.Children.Add(row);
			first = false;
		}
		return Card(stack, new Thickness(0));
	}

	private static View Padded(View content) => new Grid
	{
		Padding = new Thickness(16, 14),
		Children = { content }
	};

	private static Button TextButton(string text, Action onTap, Color color) => new()
	{
		Text = text,
		FontSize = 14,
		FontFamily = Theme.SemiBold,
		TextColor = color,
		BackgroundColor = Colors.Transparent,
		Padding = new Thickness(8, 0),
		MinimumHeightRequest = 44,
		Command = new Command(onTap)
	};

	private static Button PrimaryButton(string text, Action onTap) => new()
	{
		Text = text,
		FontSize = 15,
		FontFamily = Theme.SemiBold,
		TextColor = Theme.OnAccent,
		BackgroundColor = Theme.Accent,
		CornerRadius = 8,
		HeightRequest = 48,
		HorizontalOptions = LayoutOptions.Fill,
		Command = new Command(onTap)
	};

	private static View IconBadge(string glyph) => new Border
	{
		WidthRequest = 40,
		HeightRequest = 40,
		BackgroundColor = Theme.SurfaceRaised,
		StrokeThickness = 0,
		StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(Theme.RadiusControl) },
		VerticalOptions = LayoutOptions.Start,
		Content = IconView(glyph, Theme.Accent, 20)
	};

	// ---------------------------------------------------------------- Lignes de liste

	private static View EventRow(CampusEvent item, string meta)
	{
		var time = Text(item.Time, 14, Theme.Accent, true);
		time.WidthRequest = 52;

		var details = new VerticalStackLayout
		{
			Spacing = 3,
			Children =
			{
				Text(item.Title, 15, Theme.TextPrimary, true),
				Text(item.Place, 13, Theme.TextSecondary),
				Text(meta, 12, Theme.TextMuted)
			}
		};

		var row = new Grid
		{
			ColumnSpacing = 12,
			ColumnDefinitions = { new ColumnDefinition(GridLength.Auto), new ColumnDefinition(GridLength.Star) },
			Children = { time, details }
		};
		Grid.SetColumn(details, 1);
		return Padded(row);
	}

	private static View AlertRow((string Category, string Title, string Message, string Time, bool Urgent) alert)
	{
		var accent = alert.Urgent ? Theme.Danger : Theme.Accent;

		var category = Text(alert.Category.ToUpperInvariant(), 11, accent, true);
		category.CharacterSpacing = 0.5;
		var time = Text(alert.Time, 12, Theme.TextMuted);

		var header = new Grid
		{
			ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) },
			Children = { category, time }
		};
		Grid.SetColumn(time, 1);

		return Padded(new VerticalStackLayout
		{
			Spacing = 4,
			Children =
			{
				header,
				Text(alert.Title, 15, Theme.TextPrimary, true),
				Text(alert.Message, 13, Theme.TextSecondary)
			}
		});
	}

	private static View PlaceRow((string Category, string Title, string Detail, string Icon) place)
	{
		var texts = new VerticalStackLayout
		{
			Spacing = 3,
			VerticalOptions = LayoutOptions.Center,
			Children = { Text(place.Title, 15, Theme.TextPrimary, true), Text(place.Detail, 13, Theme.TextSecondary) }
		};

		var row = new Grid
		{
			ColumnSpacing = 14,
			ColumnDefinitions = { new ColumnDefinition(GridLength.Auto), new ColumnDefinition(GridLength.Star) },
			Children = { IconBadge(Icons.FromName(place.Icon)), texts }
		};
		Grid.SetColumn(texts, 1);
		return Padded(row);
	}

	// ---------------------------------------------------------------- Utilitaires

	private static string Cap(string text) =>
		text.Length == 0 ? text : char.ToUpper(text[0], Fr) + text[1..];

}
