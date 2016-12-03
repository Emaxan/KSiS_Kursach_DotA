using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DotA.Forms
{
	internal sealed class Circle : Form
	{
		private const double Radius = 7.5;

		public Circle(Color color) : base(color)
		{
		}

		protected override void Create() => 
			Application.Current.Dispatcher.Invoke(() =>
			Obj = new Ellipse
			{
				Stroke = new SolidColorBrush(Color),
				StrokeThickness = 1,
				Fill = new SolidColorBrush(Color),
				Width = 2*Radius,
				Height = 2*Radius,
				RenderTransform = new TranslateTransform(-100, -100)
			});

		public override void SetPosition(int x, int y) =>
			Application.Current.Dispatcher.Invoke(() =>
			{
				((Ellipse) Obj).RenderTransform = new TranslateTransform(x - Radius, y - Radius);
				((Ellipse) Obj).Stroke = new SolidColorBrush(Color);
				((Ellipse) Obj).Fill = new SolidColorBrush(Color);
			});
	}
}