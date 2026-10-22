using CampuGO.Services;
using Microsoft.Maui.Controls.Shapes;

namespace CampuGO;

public partial class MainPage
{
	/// <summary>Champ de formulaire : libellé, saisie encadrée et message d'erreur.</summary>
	private sealed class Field
	{
		public Field(string title, View input)
		{
			Box = new Border
			{
				Padding = new Thickness(12, 0),
				MinimumHeightRequest = 48,
				BackgroundColor = Theme.Surface,
				Stroke = Theme.Border,
				StrokeThickness = 1,
				StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(Theme.RadiusControl) },
				Content = input
			};
			Error = Text(string.Empty, 12, Theme.Danger);
			Error.IsVisible = false;
			Root = new VerticalStackLayout
			{
				Spacing = 6,
				Children = { Text(title, 13, Theme.TextSecondary, true), Box, Error }
			};
		}

		public Border Box { get; }
		public Label Error { get; }
		public VerticalStackLayout Root { get; }

		public bool Check(string? message)
		{
			Error.Text = message ?? string.Empty;
			Error.IsVisible = message is not null;
			Box.Stroke = message is null ? Theme.Border : Theme.Danger;
			return message is null;
		}
	}

	private void BuildSignup()
	{
		var name = FormEntry("Ex. Brice Ekambi", Keyboard.Text);
		var email = FormEntry("nom@exemple.com", Keyboard.Email);
		var phone = FormEntry("6XX XX XX XX", Keyboard.Telephone);
		var institution = FormPicker("Choisir un établissement", CampusData.InstitutionLabels);
		var track = FormPicker("Choisir une filière", Array.Empty<string>());
		var level = FormPicker("Choisir un niveau", CampusData.Levels);
		var campus = FormPicker("Choisir un campus", CampusData.Campuses);
		var password = FormEntry("8 caractères minimum", Keyboard.Default, true);
		var confirm = FormEntry("Retapez le mot de passe", Keyboard.Default, true);

		var fName = new Field("Nom complet", name);
		var fEmail = new Field("Adresse e-mail", email);
		var fPhone = new Field("Téléphone (facultatif)", phone);
		var fInstitution = new Field("Établissement", institution);
		var fTrack = new Field("Filière", track);
		var fLevel = new Field("Niveau", level);
		var fCampus = new Field("Campus", campus);

		institution.SelectedIndexChanged += (_, _) =>
		{
			if (institution.SelectedIndex < 0) return;
			var selected = CampusData.Institutions[institution.SelectedIndex];
			track.ItemsSource = selected.Tracks.ToList();
			track.SelectedIndex = -1;
			var campusIndex = Array.IndexOf(CampusData.Campuses, selected.DefaultCampus);
			if (campusIndex >= 0) campus.SelectedIndex = campusIndex;
		};

		// Mot de passe : bouton œil, indicateur de robustesse, confirmation
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
			confirm.IsPassword = password.IsPassword;
			eye.Source = Icons.Source(password.IsPassword ? Icons.Visibility : Icons.VisibilityOff, Theme.TextMuted, 22);
		};

		var passwordRow = new Grid
		{
			ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) },
			Children = { password, eye }
		};
		Grid.SetColumn(eye, 1);

		var fPassword = new Field("Mot de passe", passwordRow);
		var fConfirm = new Field("Confirmation du mot de passe", confirm);

		var bars = Enumerable.Range(0, 4).Select(_ => new BoxView { HeightRequest = 4, CornerRadius = 2, Color = Theme.SurfaceRaised }).ToArray();
		var barsGrid = new Grid { ColumnSpacing = 4 };
		for (var i = 0; i < bars.Length; i++)
		{
			barsGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
			Grid.SetColumn(bars[i], i);
			barsGrid.Children.Add(bars[i]);
		}
		var strength = Text("Robustesse : —", 12, Theme.TextMuted);
		password.TextChanged += (_, args) => UpdateStrength(args.NewTextValue ?? string.Empty, bars, strength);

		// Consentement
		var consent = new CheckBox { Color = Theme.Accent, VerticalOptions = LayoutOptions.Start };
		var consentText = Text("J’accepte que ces informations soient enregistrées sur cet appareil pour créer mon profil.", 13, Theme.TextSecondary);
		var consentTap = new TapGestureRecognizer();
		consentTap.Tapped += (_, _) => consent.IsChecked = !consent.IsChecked;
		consentText.GestureRecognizers.Add(consentTap);
		var consentRow = new Grid
		{
			ColumnSpacing = 4,
			ColumnDefinitions = { new ColumnDefinition(GridLength.Auto), new ColumnDefinition(GridLength.Star) },
			Children = { consent, consentText }
		};
		Grid.SetColumn(consentText, 1);
		consentText.VerticalOptions = LayoutOptions.Center;
		var errConsent = Text(string.Empty, 12, Theme.Danger);
		errConsent.IsVisible = false;

		var summary = Text(string.Empty, 13, Theme.Danger);

		bool Validate()
		{
			var ok = true;

			var fullName = name.Text?.Trim() ?? string.Empty;
			ok &= fName.Check(fullName.Length < 3 ? "Saisissez votre nom complet." : null);

			var mail = email.Text?.Trim() ?? string.Empty;
			var mailOk = System.Net.Mail.MailAddress.TryCreate(mail, out _) && mail.Contains('@') && mail[(mail.IndexOf('@') + 1)..].Contains('.');
			ok &= fEmail.Check(mailOk ? null : "Saisissez une adresse e-mail valide.");

			var phoneText = phone.Text?.Trim() ?? string.Empty;
			ok &= fPhone.Check(phoneText.Length == 0 || NormalizePhone(phoneText) is not null
				? null : "Numéro invalide : 9 chiffres commençant par 6 (ex. 6 77 12 34 56).");

			ok &= fInstitution.Check(institution.SelectedIndex < 0 ? "Choisissez votre établissement." : null);
			ok &= fTrack.Check(track.SelectedIndex < 0 ? "Choisissez votre filière." : null);
			ok &= fLevel.Check(level.SelectedIndex < 0 ? "Choisissez votre niveau." : null);
			ok &= fCampus.Check(campus.SelectedIndex < 0 ? "Choisissez votre campus." : null);

			var pwd = password.Text ?? string.Empty;
			var pwdOk = pwd.Length >= 8 && pwd.Any(char.IsDigit) && pwd.Any(char.IsUpper);
			ok &= fPassword.Check(pwdOk ? null : "8 caractères minimum, avec un chiffre et une majuscule.");
			ok &= fConfirm.Check(pwd.Length > 0 && pwd == (confirm.Text ?? string.Empty) ? null : "Les mots de passe ne correspondent pas.");

			errConsent.Text = consent.IsChecked ? string.Empty : "Acceptez pour continuer.";
			errConsent.IsVisible = !consent.IsChecked;
			ok &= consent.IsChecked;
			return ok;
		}

		var busy = false;
		Button submit = null!;
		submit = PrimaryButton("Créer le compte", async () =>
		{
			if (busy) return;

			if (!Validate())
			{
				summary.Text = "Corrigez les champs signalés pour continuer.";
				return;
			}

			summary.Text = string.Empty;
			var selectedInstitution = CampusData.Institutions[institution.SelectedIndex];
			var draft = new UserProfile
			{
				FullName = name.Text!.Trim(),
				Email = email.Text!.Trim().ToLowerInvariant(),
				Phone = FormatPhone(NormalizePhone(phone.Text)),
				Institution = selectedInstitution.Label,
				Track = selectedInstitution.Tracks[track.SelectedIndex],
				Level = CampusData.Levels[level.SelectedIndex],
				Campus = CampusData.Campuses[campus.SelectedIndex]
			};

			busy = true;
			submit.IsEnabled = false;
			submit.Text = "Création du compte…";
			var result = await AppServices.Api.RegisterAsync(new RegisterPayload(
				draft.FullName, draft.Email, string.IsNullOrEmpty(draft.Phone) ? null : draft.Phone,
				draft.Institution, draft.Track, draft.Level, draft.Campus, password.Text!));
			busy = false;
			submit.IsEnabled = true;
			submit.Text = "Créer le compte";

			if (result.IsSuccess && result.Value is { } session)
			{
				StoreProfile(session.User);
				await DisplayAlertAsync("Compte créé", $"Bienvenue sur CampuGO, {session.User.FullName}.", "Continuer");
				ShowAfterAuth();
				return;
			}

			if (result.IsNetworkError)
			{
				var offline = await DisplayAlertAsync("Serveur injoignable",
					"Le compte ne peut pas être créé en ligne pour le moment. Enregistrer le profil sur cet appareil uniquement ?",
					"Enregistrer ici", "Annuler");
				if (!offline) return;

				_profile = draft;
				_profile.Save();
				ShowAfterAuth();
				return;
			}

			if (result.Status == 409)
			{
				fEmail.Check(result.Message);
				return;
			}

			summary.Text = result.Message ?? "Impossible de créer le compte.";
		});

		PageContent.Children.Add(FormSection("Identité", fName.Root, fEmail.Root, fPhone.Root));
		PageContent.Children.Add(FormSection("Parcours", fInstitution.Root, fTrack.Root, fLevel.Root, fCampus.Root));
		PageContent.Children.Add(FormSection("Sécurité", fPassword.Root,
			new VerticalStackLayout
			{
				Spacing = 6,
				Children = { barsGrid, strength, Text("8 caractères minimum, avec au moins un chiffre et une majuscule.", 12, Theme.TextMuted) }
			},
			fConfirm.Root));
		PageContent.Children.Add(new VerticalStackLayout { Spacing = 4, Children = { consentRow, errConsent } });
		PageContent.Children.Add(new VerticalStackLayout
		{
			Spacing = 10,
			Children = { summary, submit, TextButton("Déjà un compte ? Se connecter", () => NavigateTo(Screen.Login), Theme.Accent) }
		});
	}

	private static View FormSection(string title, params View[] fields)
	{
		var label = Text(title.ToUpperInvariant(), 12, Theme.TextMuted, true);
		label.CharacterSpacing = 0.6;

		var stack = new VerticalStackLayout { Spacing = 16 };
		stack.Children.Add(label);
		foreach (var field in fields) stack.Children.Add(field);
		return stack;
	}

	private static Entry FormEntry(string placeholder, Keyboard keyboard, bool isPassword = false) => new()
	{
		Placeholder = placeholder,
		PlaceholderColor = Theme.TextMuted,
		TextColor = Theme.TextPrimary,
		FontFamily = Theme.Regular,
		FontSize = 15,
		BackgroundColor = Colors.Transparent,
		Keyboard = keyboard,
		IsPassword = isPassword,
		HeightRequest = 48
	};

	private static Picker FormPicker(string title, IEnumerable<string> items) => new()
	{
		Title = title,
		TitleColor = Theme.TextMuted,
		TextColor = Theme.TextPrimary,
		FontFamily = Theme.Regular,
		FontSize = 15,
		BackgroundColor = Colors.Transparent,
		HeightRequest = 48,
		ItemsSource = items.ToList()
	};

	private static string? NormalizePhone(string? raw)
	{
		var digits = new string((raw ?? string.Empty).Where(char.IsDigit).ToArray());
		if (digits.Length == 12 && digits.StartsWith("237", StringComparison.Ordinal)) digits = digits[3..];
		return digits.Length == 9 && digits[0] == '6' ? digits : null;
	}

	private static string FormatPhone(string? digits) => digits is null
		? string.Empty
		: $"+237 {digits[..3]} {digits.Substring(3, 2)} {digits.Substring(5, 2)} {digits.Substring(7, 2)}";

	private static int PasswordScore(string password) =>
		(password.Length >= 8 ? 1 : 0) +
		(password.Any(char.IsDigit) ? 1 : 0) +
		(password.Any(char.IsUpper) ? 1 : 0) +
		(password.Any(c => !char.IsLetterOrDigit(c)) ? 1 : 0);

	private static void UpdateStrength(string password, BoxView[] bars, Label label)
	{
		var score = password.Length == 0 ? 0 : PasswordScore(password);
		var color = score switch { 1 => Theme.Danger, 2 => Theme.Warning, _ => Theme.Accent };
		for (var i = 0; i < bars.Length; i++) bars[i].Color = i < score ? color : Theme.SurfaceRaised;

		label.Text = score switch
		{
			0 => "Robustesse : —",
			1 => "Robustesse : faible",
			2 => "Robustesse : moyenne",
			3 => "Robustesse : solide",
			_ => "Robustesse : très solide"
		};
		label.TextColor = score == 0 ? Theme.TextMuted : color;
	}
}
