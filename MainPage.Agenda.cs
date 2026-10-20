using Microsoft.Maui.Controls.Shapes;

namespace CampuGO;

public partial class MainPage
{
	private void BuildAgenda()
	{
		var days = Enumerable.Range(0, 7).Select(offset => DateTime.Today.AddDays(offset)).ToList();
		if (_agendaDate.Date < days[0] || _agendaDate.Date > days[^1]) _agendaDate = days[0];

		var list = new VerticalStackLayout { Spacing = 12 };
		var chips = new List<(Border Box, Label Top, Label Bottom, DateTime Date)>();

		void Refresh()
		{
			foreach (var (box, top, bottom, date) in chips)
			{
				var selected = date.Date == _agendaDate.Date;
				box.BackgroundColor = selected ? Theme.Accent : Theme.Surface;
				box.Stroke = selected ? Theme.Accent : Theme.Border;
				top.TextColor = selected ? Theme.OnAccent : Theme.TextMuted;
				bottom.TextColor = selected ? Theme.OnAccent : Theme.TextPrimary;
			}

			list.Children.Clear();
			list.Children.Add(Text(Cap(_agendaDate.ToString("dddd d MMMM", Fr)), 16, Theme.TextPrimary, true));

			var events = CampusData.EventsOn(_agendaDate);
			if (events.Count == 0)
				list.Children.Add(Text("Aucun événement prévu ce jour.", 14, Theme.TextSecondary));
			else
				list.Children.Add(ListCard(events.Select(item => EventRow(item, item.Category))));
		}

		var dayRow = new HorizontalStackLayout { Spacing = 8 };
		foreach (var date in days)
		{
			var captured = date;
			var top = Text(date == days[0] ? "Auj." : Cap(date.ToString("ddd", Fr).TrimEnd('.')), 12, Theme.TextMuted, true, TextAlignment.Center);
			var bottom = Text(date.Day.ToString(), 17, Theme.TextPrimary, true, TextAlignment.Center);

			var box = new Border
			{
				WidthRequest = 56,
				Padding = new Thickness(0, 10),
				BackgroundColor = Theme.Surface,
				Stroke = Theme.Border,
				StrokeThickness = 1,
				StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(Theme.RadiusControl) },
				Content = new VerticalStackLayout { Spacing = 2, Children = { top, bottom } }
			};
			AutomationProperties.SetName(box, Cap(date.ToString("dddd d MMMM", Fr)));

			var tap = new TapGestureRecognizer();
			tap.Tapped += (_, _) => { _agendaDate = captured; Refresh(); };
			box.GestureRecognizers.Add(tap);

			chips.Add((box, top, bottom, date));
			dayRow.Children.Add(box);
		}

		PageContent.Children.Add(new ScrollView
		{
			Orientation = ScrollOrientation.Horizontal,
			HorizontalScrollBarVisibility = ScrollBarVisibility.Never,
			Content = dayRow
		});
		PageContent.Children.Add(list);
		Refresh();
	}
}
