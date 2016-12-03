using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DotA.Forms
{
	internal sealed class Star : Form
	{
		private const int Width = 15, Height = 15;

		public Star(Color color) : base(color)
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
					// ReSharper disable PossibleLossOfFraction
					new Point(Width/2, 0),
					new Point(Width*3/5, Height*2/5),
					new Point(Width, Height*2/5),
					new Point(Width*7/10, Height*3/5),
					new Point(Width*4/5, Height),
					new Point(Width/2, Height*4/5),
					new Point(Width/5, Height),
					new Point(Width*3/10, Height*3/5),
					new Point(0, Height*2/5),
					new Point(Width*2/5, Height*2/5)
					// ReSharper restore PossibleLossOfFraction
				}),
				RenderTransform = new TranslateTransform(-100, -100)
			});

		public override void SetPosition(int x, int y) => 
			Application.Current.Dispatcher.Invoke(() =>
			{
				((Polygon) Obj).RenderTransform = new TranslateTransform(x - Width/2, y - Height/2);
				((Polygon)Obj).Stroke = new SolidColorBrush(Color);
				((Polygon)Obj).Fill = new SolidColorBrush(Color);
			});
	}
}