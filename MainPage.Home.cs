using Microsoft.Maui.Controls.Shapes;

namespace CampuGO;

public partial class MainPage
{
	private void BuildHome()
	{
		PageContent.Children.Add(CampusPhoto());

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
	}

	/// <summary>Photographie du campus de l'ENSPD (fichier Resources/Images/enspd_campus.jpg).</summary>
	private static View CampusPhoto()
	{
		var photo = new Image
		{
			Source = "enspd_campus",
			Aspect = Aspect.AspectFill,
			HeightRequest = 220
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
