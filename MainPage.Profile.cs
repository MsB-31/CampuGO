using Microsoft.Maui.Controls.Shapes;

namespace CampuGO;

public partial class MainPage
{
	private void BuildProfile()
	{
		if (_profile is null)
		{
			PageContent.Children.Add(Card(new VerticalStackLayout
			{
				Spacing = 12,
				Children =
				{
					Text("Aucun compte créé", 16, Theme.TextPrimary, true),
					Text("Créez un compte pour enregistrer votre établissement, votre filière et votre campus sur cet appareil.", 14, Theme.TextSecondary),
					PrimaryButton("Créer un compte", () => NavigateTo(Screen.Signup))
				}
			}));
			return;
		}

		var initial = Text(_profile.Initial, 22, Theme.Accent, true, TextAlignment.Center);
		initial.VerticalOptions = LayoutOptions.Center;

		var avatar = new Border
		{
			WidthRequest = 56,
			HeightRequest = 56,
			BackgroundColor = Theme.SurfaceRaised,
			StrokeThickness = 0,
			StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(28) },
			Content = initial
		};

		var identity = new VerticalStackLayout
		{
			Spacing = 2,
			VerticalOptions = LayoutOptions.Center,
			Children = { Text(_profile.FullName, 17, Theme.TextPrimary, true), Text(_profile.Email, 13, Theme.TextSecondary) }
		};

		var header = new Grid
		{
			ColumnSpacing = 16,
			ColumnDefinitions = { new ColumnDefinition(GridLength.Auto), new ColumnDefinition(GridLength.Star) },
			Children = { avatar, identity }
		};
		Grid.SetColumn(identity, 1);
		PageContent.Children.Add(header);

		var rows = new List<View>
		{
			DetailRow("Établissement", _profile.Institution),
			DetailRow("Filière", _profile.Track),
			DetailRow("Niveau", _profile.Level),
			DetailRow("Campus", _profile.Campus)
		};
		if (!string.IsNullOrWhiteSpace(_profile.Phone)) rows.Add(DetailRow("Téléphone", _profile.Phone));
		PageContent.Children.Add(ListCard(rows));

		PageContent.Children.Add(TextButton("Supprimer le profil de cet appareil", async () =>
		{
			var confirmed = await DisplayAlertAsync("Supprimer le profil",
				"Les informations enregistrées sur cet appareil seront effacées.", "Supprimer", "Annuler");
			if (!confirmed) return;
			UserProfile.Clear();
			_profile = null;
			Show(Screen.Profile);
		}, Theme.Danger));
	}

	private static View DetailRow(string label, string value) => Padded(new VerticalStackLayout
	{
		Spacing = 2,
		Children = { Text(label, 12, Theme.TextMuted), Text(value, 15, Theme.TextPrimary) }
	});
}
