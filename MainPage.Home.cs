using Microsoft.Maui.Controls.Shapes;

namespace CampuGO;

public partial class MainPage
{
	private const double PhotoHeight = 300;
	private const double PhotoBlockHeight = PhotoHeight + 8 + 16;   // photo + espace + légende

	private void BuildHome()
	{
		_homePhoto = CampusPhoto();
		PageContent.Children.Add(_homePhoto);

		View account;
		if (_profile is null)
		{
			account = Card(new VerticalStackLayout
			{
				Spacing = 12,
				Children =
				{
					Text("Créer un compte étudiant", 16, Theme.TextPrimary, true),
					Text("Renseignez votre établissement, votre filière et votre campus pour personnaliser l’application.", 14, Theme.TextSecondary),
					PrimaryButton("Créer un compte", () => NavigateTo(Screen.Signup))
				}
			});
		}
		else
		{
			account = new VerticalStackLayout
			{
				Spacing = 2,
				Children =
				{
					Text($"Bonjour, {_profile.FullName}", 18, Theme.TextPrimary, true),
					Text($"{_profile.Track} · {_profile.Level} · {_profile.Campus}", 13, Theme.TextSecondary)
				}
			};
		}

		account.Margin = new Thickness(0, 8, 0, 0);
		PageContent.Children.Add(account);
		CenterHomePhoto();
	}

	/// <summary>Centre la photo dans la zone visible : la carte de compte se trouve juste sous le bas de l'écran, un léger défilement la révèle.</summary>
	private void CenterHomePhoto()
	{
		if (_current != Screen.Home || _homePhoto is null) return;
		var available = ContentScroll.Height;
		var top = available > 0 ? Math.Max(24, (available - 8 - PhotoBlockHeight) / 2) : 72;
		_homePhoto.Margin = new Thickness(0, top, 0, 0);
	}

	/// <summary>Photographie du campus de l'ENSPD (fichier Resources/Images/enspd_campus.jpg).</summary>
	private static View CampusPhoto()
	{
		var photo = new Image
		{
			Source = "enspd_campus",
			Aspect = Aspect.AspectFill,
			HeightRequest = PhotoHeight
		};
		AutomationProperties.SetName(photo, "Campus de l’ENSPD");

		var frame = new Border
		{
			Padding = 0,
			BackgroundColor = Theme.Surface,
			Stroke = Theme.Border,
			StrokeThickness = 1,
			StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(Theme.RadiusCard) },
			Content = photo
		};

		return new VerticalStackLayout
		{
			Spacing = 8,
			Children = { frame, Text("Campus de l’ENSPD", 12, Theme.TextMuted) }
		};
	}
}
