namespace CampuGO;

public partial class MainPage
{
	private void BuildSignupPage()
	{
		PageContent.Children.Add(CreateHeader());
		PageContent.Children.Add(PageTitle("Créer mon compte", "Rejoins la communauté étudiante de l’Université de Douala."));

		// Identité
		var name = CreateFormEntry("Ex. Brice Ekambi", Keyboard.Text);
		var email = CreateFormEntry("nom@exemple.com", Keyboard.Email);
		var phone = CreateFormEntry("6XX XX XX XX (facultatif)", Keyboard.Telephone);
		var errName = ErrorLabel();
		var errEmail = ErrorLabel();
		var errPhone = ErrorLabel();

		// Parcours
		var institution = CreatePicker("Choisir un établissement", CampusData.InstitutionLabels);
		var track = CreatePicker("Choisir une filière", Array.Empty<string>());
		var level = CreatePicker("Choisir un niveau", CampusData.Levels);
		var campus = CreatePicker("Choisir un campus", CampusData.Campuses);
		var errInstitution = ErrorLabel();
		var errTrack = ErrorLabel();
		var errLevel = ErrorLabel();
		var errCampus = ErrorLabel();

		institution.SelectedIndexChanged += (_, _) =>
		{
			var selected = institution.SelectedIndex < 0 ? null : CampusData.Institutions[institution.SelectedIndex];
			if (selected is null) return;
			track.ItemsSource = selected.Tracks.ToList();
			track.SelectedIndex = -1;
			var campusIndex = Array.IndexOf(CampusData.Campuses, selected.DefaultCampus);
			if (campusIndex >= 0) campus.SelectedIndex = campusIndex;
		};

		// Sécurité
		var password = CreateFormEntry("8 caractères minimum", Keyboard.Default, isPassword: true);
		var confirm = CreateFormEntry("Retape ton mot de passe", Keyboard.Default, isPassword: true);
		var errPassword = ErrorLabel();
		var errConfirm = ErrorLabel();
		var toggle = new Button
		{
			Text = "Afficher", FontSize = 12, FontFamily = "OpenSansSemibold", TextColor = Teal,
			BackgroundColor = Colors.Transparent, Padding = new Thickness(8, 0), MinimumHeightRequest = 44
		};
		toggle.Clicked += (_, _) =>
		{
			password.IsPassword = !password.IsPassword;
			confirm.IsPassword = password.IsPassword;
			toggle.Text = password.IsPassword ? "Afficher" : "Masquer";
		};
		var passwordRow = new Grid { ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) } };
		passwordRow.Children.Add(password);
		passwordRow.Children.Add(toggle);
		Grid.SetColumn(toggle, 1);

		var bars = Enumerable.Range(0, 4).Select(_ => new BoxView { HeightRequest = 4, CornerRadius = 2, Color = BarOff }).ToArray();
		var barsGrid = new Grid { ColumnSpacing = 4 };
		for (var i = 0; i < bars.Length; i++)
		{
			barsGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
			Grid.SetColumn(bars[i], i);
			barsGrid.Children.Add(bars[i]);
		}
		var strength = Text("Robustesse : —", 12, MutedText);
		password.TextChanged += (_, args) => UpdateStrength(args.NewTextValue ?? string.Empty, bars, strength);

		var consent = new CheckBox { Color = Teal, MinimumHeightRequest = 44, MinimumWidthRequest = 44 };
		var errConsent = ErrorLabel();
		var summary = Text(string.Empty, 13, AlertColor);

		PageContent.Children.Add(SectionLabel("IDENTITÉ"));
		PageContent.Children.Add(FormField("Nom complet", name, errName));
		PageContent.Children.Add(FormField("Adresse e-mail", email, errEmail));
		PageContent.Children.Add(FormField("Téléphone (+237)", phone, errPhone));
		PageContent.Children.Add(SectionLabel("PARCOURS"));
		PageContent.Children.Add(FormField("Établissement", institution, errInstitution));
		PageContent.Children.Add(FormField("Filière", track, errTrack));
		PageContent.Children.Add(FormField("Niveau", level, errLevel));
		PageContent.Children.Add(FormField("Campus", campus, errCampus));
		PageContent.Children.Add(SectionLabel("SÉCURITÉ"));
		PageContent.Children.Add(FormField("Mot de passe", passwordRow, errPassword));
		PageContent.Children.Add(new VerticalStackLayout
		{
			Spacing = 6,
			Children = { barsGrid, strength, Text("8 caractères minimum, avec au moins un chiffre et une majuscule.", 11, MutedText) }
		});
		PageContent.Children.Add(FormField("Confirmer le mot de passe", confirm, errConfirm));
		PageContent.Children.Add(new VerticalStackLayout
		{
			Spacing = 4,
			Children =
			{
				new HorizontalStackLayout
				{
					Spacing = 8,
					Children = { consent, Text("J’accepte l’utilisation de ces informations pour créer mon profil.", 12, SecondaryText) }
				},
				errConsent
			}
		});

		bool Validate()
		{
			var ok = true;
			var fullName = name.Text?.Trim() ?? string.Empty;
			ok &= Check(errName, fullName.Length < 3 ? "Saisis ton nom complet." : null);

			var mail = email.Text?.Trim() ?? string.Empty;
			var mailOk = System.Net.Mail.MailAddress.TryCreate(mail, out _) && mail.Contains('@') && mail[(mail.IndexOf('@') + 1)..].Contains('.');
			ok &= Check(errEmail, mailOk ? null : "Saisis une adresse e-mail valide.");

			var phoneText = phone.Text?.Trim() ?? string.Empty;
			ok &= Check(errPhone, phoneText.Length == 0 || NormalizePhone(phoneText) is not null
				? null : "Numéro invalide : 9 chiffres commençant par 6 (ex. 6 77 12 34 56).");

			ok &= Check(errInstitution, institution.SelectedIndex < 0 ? "Choisis ton établissement." : null);
			ok &= Check(errTrack, track.SelectedIndex < 0 ? "Choisis ta filière." : null);
			ok &= Check(errLevel, level.SelectedIndex < 0 ? "Choisis ton niveau." : null);
			ok &= Check(errCampus, campus.SelectedIndex < 0 ? "Choisis ton campus." : null);

			var pwd = password.Text ?? string.Empty;
			var pwdOk = pwd.Length >= 8 && pwd.Any(char.IsDigit) && pwd.Any(char.IsUpper);
			ok &= Check(errPassword, pwdOk ? null : "8 caractères minimum, avec un chiffre et une majuscule.");
			ok &= Check(errConfirm, pwd == (confirm.Text ?? string.Empty) && pwd.Length > 0 ? null : "Les mots de passe ne correspondent pas.");
			ok &= Check(errConsent, consent.IsChecked ? null : "Accepte les conditions pour continuer.");
			return ok;
		}

		var submit = new Button
		{
			Text = "Créer mon compte",
			FontSize = 15,
			FontFamily = "OpenSansSemibold",
			TextColor = OnTeal,
			BackgroundColor = Teal,
			CornerRadius = 11,
			MinimumHeightRequest = 52,
			Command = new Command(async () =>
			{
				if (!Validate())
				{
					summary.Text = "Corrige les champs signalés pour continuer.";
					return;
				}

				summary.Text = string.Empty;
				await DisplayAlertAsync("Compte créé", $"Bienvenue sur CampuGO, {name.Text!.Trim()} !", "Continuer");
				ShowTab("Accueil");
			})
		};
		PageContent.Children.Add(summary);
		PageContent.Children.Add(submit);
		PageContent.Children.Add(new Button
		{
			Text = "Retour à l’accueil",
			FontSize = 14,
			TextColor = Teal,
			BackgroundColor = Colors.Transparent,
			MinimumHeightRequest = 44,
			Command = new Command(() => ShowTab("Accueil"))
		});
	}

	private static bool Check(Label error, string? message)
	{
		error.Text = message ?? string.Empty;
		error.IsVisible = message is not null;
		return message is null;
	}

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
		var color = score switch { 1 => AlertColor, 2 => WarnColor, _ => Teal };
		for (var i = 0; i < bars.Length; i++) bars[i].Color = i < score ? color : BarOff;
		label.Text = score switch
		{
			0 => "Robustesse : —",
			1 => "Robustesse : faible",
			2 => "Robustesse : moyenne",
			3 => "Robustesse : solide",
			_ => "Robustesse : très solide"
		};
		label.TextColor = score == 0 ? MutedText : color;
	}

	private static Label SectionLabel(string text) => Text(text, 11, Teal, true);

	private static View FormField(string title, View input, Label error) => new VerticalStackLayout
	{
		Spacing = 6,
		Children = { Text(title, 13, SecondaryText, true), Frame(input, 10), error }
	};

	private static Label ErrorLabel() => new()
	{
		FontSize = 12, FontFamily = "OpenSansRegular", TextColor = AlertColor,
		LineBreakMode = LineBreakMode.WordWrap, IsVisible = false
	};

	private static Entry CreateFormEntry(string placeholder, Keyboard keyboard, bool isPassword = false) => new()
	{
		Placeholder = placeholder,
		PlaceholderColor = MutedText,
		TextColor = PrimaryText,
		BackgroundColor = Colors.Transparent,
		FontSize = 15,
		Keyboard = keyboard,
		IsPassword = isPassword,
		HeightRequest = 48
	};

	private static Picker CreatePicker(string title, IEnumerable<string> items) => new()
	{
		Title = title,
		TitleColor = SecondaryText,
		TextColor = PrimaryText,
		BackgroundColor = Colors.Transparent,
		FontSize = 15,
		HeightRequest = 52,
		ItemsSource = items.ToList()
	};
}
