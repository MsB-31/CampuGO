using Microsoft.Maui.Controls.Shapes;

namespace CampuGO;

public partial class MainPage : ContentPage
{
	private static readonly Color Surface = Color.FromArgb("#192326");
	private static readonly Color Teal = Color.FromArgb("#45D6C2");
	private static readonly Color TealDark = Color.FromArgb("#173B38");
	private static readonly Color PrimaryText = Color.FromArgb("#F4F7F6");
	private static readonly Color SecondaryText = Color.FromArgb("#AAB8B5");
	private static readonly Color MutedText = Color.FromArgb("#71817E");
	private static readonly Color AlertColor = Color.FromArgb("#FF806F");
	private static readonly Color WarnColor = Color.FromArgb("#F5B942");
	private static readonly Color BarOff = Color.FromArgb("#293638");
	private static readonly Color OnTeal = Color.FromArgb("#101719");

	private string _activeTab = "Accueil";
	private string _selectedFilter = "Tout";
	private string _searchQuery = string.Empty;

	public MainPage()
	{
		InitializeComponent();
		ShowTab("Accueil");
	}

	private void ShowTab(string tab)
	{
		_activeTab = tab;
		PageContent.Children.Clear();
		BottomNavigation.Children.Clear();
		BuildBottomNavigation();

		switch (tab)
		{
			case "Explorer": BuildExplorePage(); break;
			case "Agenda": BuildAgendaPage(); break;
			default: BuildHomePage(); break;
		}

		MainThread.BeginInvokeOnMainThread(async () => await ContentScroll.ScrollToAsync(0, 0, false));
	}

	private void BuildBottomNavigation()
	{
		var tabs = new[] { ("Accueil", "⌂"), ("Explorer", "⌕"), ("Agenda", "▦"), ("Alertes", "♧"), ("Profil", "◉") };
		for (var index = 0; index < tabs.Length; index++)
		{
			var tab = tabs[index];
			var selected = tab.Item1 == _activeTab;
			var item = new VerticalStackLayout
			{
				Spacing = 3,
				Padding = new Thickness(2, 5),
				HorizontalOptions = LayoutOptions.Fill,
				Children =
				{
					Text(tab.Item2, 22, selected ? Teal : SecondaryText, true, TextAlignment.Center),
					Text(tab.Item1, 11, selected ? Teal : SecondaryText, true, TextAlignment.Center)
				}
			};
			var tap = new TapGestureRecognizer();
			tap.Tapped += (_, _) => ShowTab(tab.Item1);
			item.GestureRecognizers.Add(tap);
			Grid.SetColumn(item, index);
			BottomNavigation.Children.Add(item);
		}
	}

	private View CreateHeader()
	{
		var brand = new VerticalStackLayout
		{
			Spacing = 2,
			Children =
			{
				new HorizontalStackLayout { Spacing = 0, Children = { Text("campu", 22, PrimaryText, true), Text("GO", 22, Teal, true) } },
				Text(CampusData.University.ToUpperInvariant(), 10, SecondaryText, true)
			}
		};
		var notifications = new Button
		{
			Text = "♧", FontSize = 21, TextColor = PrimaryText, BackgroundColor = Surface,
			CornerRadius = 14, WidthRequest = 48, HeightRequest = 48, Padding = 0,
			Command = new Command(() => ShowTab("Alertes"))
		};
		AutomationProperties.SetName(notifications, "Ouvrir les alertes");
		var row = new Grid
		{
			ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) },
			Children = { brand, notifications }
		};
		Grid.SetColumn(notifications, 1);
		return row;
	}

	private View PageTitle(string title, string subtitle) => new VerticalStackLayout
	{
		Spacing = 5,
		Children = { Text(title, 27, PrimaryText, true), Text(subtitle, 14, SecondaryText) }
	};

	private View Section(string title, string action, Action onTap)
	{
		var titleLabel = Text(title, 18, PrimaryText, true);
		var button = new Button
		{
			Text = action, FontSize = 13, FontFamily = "OpenSansSemibold", TextColor = Teal,
			BackgroundColor = Colors.Transparent, Padding = new Thickness(8, 4), MinimumHeightRequest = 36,
			Command = new Command(onTap)
		};
		var row = new Grid
		{
			ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) },
			Children = { titleLabel, button }
		};
		Grid.SetColumn(button, 1);
		return row;
	}

	private View EventCard(string date, string title, string detail, string action, Action onTap) => Frame(new VerticalStackLayout
	{
		Spacing = 9,
		Children = { Text(date, 10, Teal, true), Text(title, 17, PrimaryText, true), Text(detail, 12, SecondaryText), ActionButton(action, onTap, false) }
	}, 15);

	private View InfoCard(string icon, string title, string detail) => Frame(new HorizontalStackLayout
	{
		Spacing = 13,
		Children =
		{
			Frame(Text(icon, 20, Teal, true, TextAlignment.Center), 10, TealDark),
			new VerticalStackLayout { Spacing = 4, VerticalOptions = LayoutOptions.Center, Children = { Text(title, 14, PrimaryText, true), Text(detail, 12, SecondaryText) } }
		}
	}, 14);

	private Button ActionButton(string title, Action onTap, bool primary) => new()
	{
		Text = title, FontSize = 14, FontFamily = "OpenSansSemibold",
		TextColor = primary ? OnTeal : Teal,
		BackgroundColor = primary ? Teal : TealDark, CornerRadius = 11,
		MinimumHeightRequest = 48, Padding = new Thickness(16, 10), HorizontalOptions = LayoutOptions.Start,
		Command = new Command(onTap)
	};

	private new static Border Frame(View content, double padding, Color? background = null) => new()
	{
		Padding = padding, BackgroundColor = background ?? Surface,
		Stroke = Color.FromArgb("#293638"), StrokeThickness = 1,
		StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(15) }, Content = content
	};

	private static Label Text(string text, double size, Color color, bool semibold = false, TextAlignment alignment = TextAlignment.Start) => new()
	{
		Text = text, FontSize = size, FontFamily = semibold ? "OpenSansSemibold" : "OpenSansRegular",
		TextColor = color, LineBreakMode = LineBreakMode.WordWrap, VerticalOptions = LayoutOptions.Center,
		HorizontalTextAlignment = alignment
	};
}
