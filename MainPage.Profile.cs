namespace CampuGO;

public partial class MainPage
{
	private void BuildProfilePage()
	{
		PageContent.Children.Add(CreateHeader());
		PageContent.Children.Add(PageTitle("Mon profil", "Ton espace personnel CampuGO."));

		PageContent.Children.Add(InfoCard("◉", "Compte étudiant", "Université de Douala · ENSPD"));

		PageContent.Children.Add(Section("Mon espace", "", () => { }));
		PageContent.Children.Add(ProfileRow("▦", "Mes événements", "Retrouver tes prochaines sorties", () => ShowTab("Agenda")));
		PageContent.Children.Add(ProfileRow("♧", "Mes notifications", "Gérer les informations reçues", () => ShowTab("Alertes")));
		PageContent.Children.Add(ProfileRow("⚙", "Préférences", "Langue, confidentialité et aide", async () => await DisplayAlertAsync("Préférences", "Tes préférences seront disponibles dans une prochaine version.", "Fermer")));

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
