using System.Globalization;

namespace CampuGO;

public partial class MainPage : ContentPage
{
	private enum Screen { Home, Explore, Agenda, Alerts, Profile, Signup }

	private static readonly (Screen Target, string Label, string Icon)[] Tabs =
	{
		(Screen.Home, "Accueil", Icons.Home),
		(Screen.Explore, "Explorer", Icons.Search),
		(Screen.Agenda, "Agenda", Icons.Calendar),
		(Screen.Alerts, "Alertes", Icons.Bell),
		(Screen.Profile, "Profil", Icons.Person)
	};

	private static readonly CultureInfo Fr = new("fr-FR");

	private readonly List<Screen> _history = new();
	private Screen _current = Screen.Home;
	private UserProfile? _profile = UserProfile.Load();
	private string _exploreFilter = "Tout";
	private string _exploreQuery = string.Empty;
	private DateTime _agendaDate = DateTime.Today;

	public MainPage()
	{
		InitializeComponent();
		BackgroundColor = Theme.Background;
		BottomBar.BackgroundColor = Theme.Surface;
		BottomBarLine.Color = Theme.Border;
		Show(Screen.Home);
	}

	// ---------------------------------------------------------------- Navigation

	/// <summary>Ouvre un écran et mémorise l'écran actuel pour pouvoir y revenir.</summary>
	private void NavigateTo(Screen target)
	{
		if (target == _current) return;
		_history.Add(_current);
		if (_history.Count > 30) _history.RemoveAt(0);
		Show(target);
	}

	/// <summary>Revient à l'écran précédent (flèche en haut à droite et bouton retour d'Android).</summary>
	private void GoBack()
	{
		if (_history.Count == 0) return;
		var previous = _history[^1];
		_history.RemoveAt(_history.Count - 1);
		Show(previous);
	}

	protected override bool OnBackButtonPressed()
	{
		if (_history.Count == 0) return base.OnBackButtonPressed();
		GoBack();
		return true;
	}

	private void Show(Screen screen)
	{
		_current = screen;
		BuildAppBar();
		BuildBottomBar();
		PageContent.Children.Clear();

		switch (screen)
		{
			case Screen.Explore: BuildExplore(); break;
			case Screen.Agenda: BuildAgenda(); break;
			case Screen.Alerts: BuildAlerts(); break;
			case Screen.Profile: BuildProfile(); break;
			case Screen.Signup: BuildSignup(); break;
			default: BuildHome(); break;
		}

		MainThread.BeginInvokeOnMainThread(async () => await ContentScroll.ScrollToAsync(0, 0, false));
	}

	private static string TitleOf(Screen screen) => screen switch
	{
		Screen.Explore => "Explorer",
		Screen.Agenda => "Agenda",
		Screen.Alerts => "Alertes",
		Screen.Profile => "Profil",
		Screen.Signup => "Créer un compte",
		_ => "CampuGO"
	};

	// ---------------------------------------------------------------- Barre du haut

	private void BuildAppBar()
	{
		AppBar.Children.Clear();

		View title;
		if (_current == Screen.Home)
		{
			title = new HorizontalStackLayout
			{
				Spacing = 0,
				Children = { Text("campu", 22, Theme.TextPrimary, true), Text("GO", 22, Theme.Accent, true) }
			};
		}
		else
		{
			title = Text(TitleOf(_current), 22, Theme.TextPrimary, true);
		}
		title.VerticalOptions = LayoutOptions.Center;
		title.Margin = new Thickness(_history.Count > 0 ? 4 : 12, 0, 0, 0);
		Grid.SetColumn(title, 1);
		AppBar.Children.Add(title);

		if (_current == Screen.Home)
		{
			var logo = new Image
			{
				Source = "enspd_logo",
				HeightRequest = 36,
				Aspect = Aspect.AspectFit,
				HorizontalOptions = LayoutOptions.End,
				VerticalOptions = LayoutOptions.Center
			};
			AutomationProperties.SetName(logo, "Logo de l’ENSPD");
			Grid.SetColumn(logo, 2);
			AppBar.Children.Add(logo);
		}

		if (_history.Count == 0) return;

		var back = new ImageButton
		{
			Source = Icons.Source(Icons.Back, Theme.TextPrimary, 24),
			BackgroundColor = Colors.Transparent,
			WidthRequest = 44,
			HeightRequest = 44,
			Padding = 10,
			VerticalOptions = LayoutOptions.Center
		};
		AutomationProperties.SetName(back, "Retour à la page précédente");
		back.Clicked += (_, _) => GoBack();
		AppBar.Children.Add(back);
	}

	// ---------------------------------------------------------------- Barre du bas

	private void BuildBottomBar()
	{
		BottomBar.IsVisible = _current != Screen.Signup;
		BottomNavigation.Children.Clear();

		for (var index = 0; index < Tabs.Length; index++)
		{
			var tab = Tabs[index];
			var selected = tab.Target == _current;
			var color = selected ? Theme.Accent : Theme.TextMuted;

			var item = new VerticalStackLayout
			{
				Spacing = 2,
				Padding = new Thickness(0, 6),
				Children =
				{
					IconView(tab.Icon, color, 24),
					Text(tab.Label, 11, color, selected, TextAlignment.Center)
				}
			};
			AutomationProperties.SetName(item, tab.Label);

			var tap = new TapGestureRecognizer();
			tap.Tapped += (_, _) => NavigateTo(tab.Target);
			item.GestureRecognizers.Add(tap);

			Grid.SetColumn(item, index);
			BottomNavigation.Children.Add(item);
		}
	}
}
