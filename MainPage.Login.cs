using CampuGO.Services;

namespace CampuGO;

public partial class MainPage
{
	private void BuildLogin()
	{
		var email = FormEntry("nom@exemple.com", Keyboard.Email);
		var password = FormEntry("Mot de passe", Keyboard.Default, true);

		var fEmail = new Field("Adresse e-mail", email);

		var eye = new ImageButton
		{
			Source = Icons.Source(Icons.Visibility, Theme.TextMuted, 22),
			BackgroundColor = Colors.Transparent,
			WidthRequest = 40,
			HeightRequest = 40,
			Padding = 8,
			VerticalOptions = LayoutOptions.Center
		};
		AutomationProperties.SetName(eye, "Afficher ou masquer le mot de passe");
		eye.Clicked += (_, _) =>
		{
			password.IsPassword = !password.IsPassword;
			eye.Source = Icons.Source(password.IsPassword ? Icons.Visibility : Icons.VisibilityOff, Theme.TextMuted, 22);
		};

		var passwordRow = new Grid
		{
			ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) },
			Children = { password, eye }
		};
		Grid.SetColumn(eye, 1);
		var fPassword = new Field("Mot de passe", passwordRow);

		var summary = Text(string.Empty, 13, Theme.Danger);
		var busy = false;
		Button submit = null!;
		submit = PrimaryButton("Se connecter", async () =>
		{
			if (busy) return;

			var ok = fEmail.Check(string.IsNullOrWhiteSpace(email.Text) ? "Saisissez votre adresse e-mail." : null);
			ok &= fPassword.Check(string.IsNullOrEmpty(password.Text) ? "Saisissez votre mot de passe." : null);
			if (!ok) return;

			summary.Text = string.Empty;
			busy = true;
			submit.IsEnabled = false;
			submit.Text = "Connexion…";
			var result = await AppServices.Api.LoginAsync(new LoginPayload(email.Text!.Trim(), password.Text!));
			busy = false;
			submit.IsEnabled = true;
			submit.Text = "Se connecter";

			if (result.IsSuccess && result.Value is { } session)
			{
				StoreProfile(session.User);
				ShowAfterAuth();
				return;
			}

			summary.Text = result.IsNetworkError
				? "Serveur injoignable. Vérifiez que l’API est lancée et que la connexion est établie."
				: result.Message ?? "Connexion impossible.";
		});

		PageContent.Children.Add(new VerticalStackLayout { Spacing = 16, Children = { fEmail.Root, fPassword.Root } });
		PageContent.Children.Add(new VerticalStackLayout
		{
			Spacing = 10,
			Children =
			{
				summary,
				submit,
				TextButton("Pas encore de compte ? Créer un compte", () => NavigateTo(Screen.Signup), Theme.Accent)
			}
		});
	}

	/// <summary>Enregistre sur l'appareil le profil reçu du serveur.</summary>
	private void StoreProfile(ApiUser user)
	{
		_profile = new UserProfile
		{
			FullName = user.FullName,
			Email = user.Email,
			Phone = user.Phone ?? string.Empty,
			Institution = user.Institution,
			Track = user.Track,
			Level = user.Level,
			Campus = user.Campus
		};
		_profile.Save();
	}

	/// <summary>Affiche le profil après une connexion ou une inscription, sans laisser les écrans de saisie dans l'historique.</summary>
	private void ShowAfterAuth()
	{
		while (_history.Count > 0 && _history[^1] is Screen.Login or Screen.Signup)
			_history.RemoveAt(_history.Count - 1);
		Show(Screen.Profile);
	}
}
