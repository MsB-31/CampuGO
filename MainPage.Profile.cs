namespace CampuGO;

public partial class MainPage
{
	private void BuildProfilePage()
	{
		PageContent.Children.Add(CreateHeader());
		PageContent.Children.Add(PageTitle("Mon profil", "Ton espace personnel CampuGO."));

		if (_profile is null)
		{
			PageContent.Children.Add(InfoCard("?", "Profil non créé", "Crée ton compte pour retrouver ton établissement,\nta filière et ton campus."));
			PageContent.Children.Add(ActionButton("Créer mon compte étudiant", () => ShowTab("Inscription"), true));
		}
		else
		{
			PageContent.Children.Add(InfoCard(_profile.Initial, _profile.FullName,
				$"{_profile.Institution}\n{_profile.Track} · {_profile.Level}\n{_profile.Campus}"));
			PageContent.Children.Add(InfoCard("✉", "Adresse e-mail", _profile.Email));
			if (!string.IsNullOrWhiteSpace(_profile.Phone))
				PageContent.Children.Add(InfoCard("☎", "Téléphone", _profile.Phone));
		}

		PageContent.Children.Add(Section("Mon espace", "", () => { }));
		PageContent.Children.Add(ProfileRow("▦", "Mes événements", "Retrouver tes prochaines sorties", () => ShowTab("Agenda")));
		PageContent.Children.Add(ProfileRow("♧", "Mes notifications", "Gérer les informations reçues", () => ShowTab("Alertes")));
		PageContent.Children.Add(ProfileRow("⚙", "Préférences", "Langue, confidentialité et aide", async () => await DisplayAlertAsync("Préférences", "Tes préférences seront disponibles dans une prochaine version.", "Fermer")));

		if (_profile is not null)
		{
			PageContent.Children.Add(new Button
			{
				Text = "Supprimer mon profil de cet appareil",
				FontSize = 13,
				TextColor = AlertColor,
				BackgroundColor = Colors.Transparent,
				MinimumHeightRequest = 44,
				Command = new Command(async () =>
				{
					var confirmed = await DisplayAlertAsync("Supprimer le profil",
						"Les informations enregistrées sur cet appareil seront effacées.", "Supprimer", "Annuler");
					if (!confirmed) return;
					UserProfile.Clear();
					_profile = null;
					ShowTab("Accueil");
				})
			});
		}
		PageContent.Children.Add(Text("CAMPUGO  ·  VERSION 1.0", 11, MutedText, true));
	}

	private View ProfileRow(string icon, string title, string subtitle, Action onTap)
	{
		var row = new HorizontalStackLayout
		{
			Spacing = 12,
			Children =
			{
				Text(icon, 20, Teal, true),
				new VerticalStackLayout { Spacing = 3, Children = { Text(title, 14, PrimaryText, true), Text(subtitle, 12, SecondaryText) } },
				Text("›", 22, SecondaryText)
			}
		};
		var tap = new TapGestureRecognizer();
		tap.Tapped += (_, _) => onTap();
		row.GestureRecognizers.Add(tap);
		return Frame(row, 14);
	}
}
