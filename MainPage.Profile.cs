using CampuGO.Services;
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
					Text("Aucun compte connecté", 16, Theme.TextPrimary, true),
					Text("Créez un compte ou connectez-vous pour enregistrer votre établissement, votre filière et votre campus.", 14, Theme.TextSecondary),
					PrimaryButton("Créer un compte", () => NavigateTo(Screen.Signup)),
					TextButton("Se connecter", () => NavigateTo(Screen.Login), Theme.Accent)
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

		var online = AppServices.Session.IsAuthenticated;
		PageContent.Children.Add(Text(
			online ? "Compte synchronisé avec le serveur." : "Profil enregistré sur cet appareil uniquement.",
			12, Theme.TextMuted));

		if (online)
		{
			PageContent.Children.Add(TextButton("Se déconnecter", async () =>
			{
				await AppServices.Api.LogoutAsync();
				UserProfile.Clear();
				_profile = null;
				_history.Clear();
				Show(Screen.Home);
			}, Theme.TextPrimary));
		}

		PageContent.Children.Add(TextButton(online ? "Supprimer mon compte" : "Supprimer le profil de cet appareil", async () =>
		{
			var serverAccount = AppServices.Session.IsAuthenticated;
			var confirmed = await DisplayAlertAsync(
				serverAccount ? "Supprimer le compte" : "Supprimer le profil",
				serverAccount
					? "Le compte et ses sessions seront supprimés du serveur. Cette action est définitive."
					: "Les informations enregistrées sur cet appareil seront effacées.",
				"Supprimer", "Annuler");
			if (!confirmed) return;

			if (serverAccount)
			{
				var result = await AppServices.Api.DeleteAccountAsync();
				if (!result.IsSuccess)
				{
					await DisplayAlertAsync("Suppression impossible",
						result.IsNetworkError ? "Serveur injoignable. Réessayez lorsque la connexion sera rétablie." : result.Message ?? "Une erreur est survenue.",
						"Fermer");
					return;
				}
			}

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
