using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DotA.Forms
{
	internal sealed class Triangle : Form
	{
		private const int Width = 15, Height = 15;

		public Triangle(Color color) : base(color)
		{

		}

		protected override void Create() =>
			Application.Current.Dispatcher.Invoke(() =>
			Obj = new Polygon
			{
				Stroke = new SolidColorBrush(Color),
				StrokeThickness = 1,
				Fill = new SolidColorBrush(Color),
				Points = new PointCollection(new List<Point>
				{
					// ReSharper disable once PossibleLossOfFraction
					new Point(Width/2, 0),
					new Point(Width, Height),
					new Point(0, Height)
				}),
				RenderTransform = new TranslateTransform(-100, -100)
			});

		public override void SetPosition(int x, int y) =>
			Application.Current.Dispatcher.Invoke(() =>
			{
				((Polygon)Obj).RenderTransform = new TranslateTransform(x - Width / 2, y - Height / 2);
				((Polygon)Obj).Stroke = new SolidColorBrush(Color);
				((Polygon)Obj).Fill = new SolidColorBrush(Color);
			});
	}
}