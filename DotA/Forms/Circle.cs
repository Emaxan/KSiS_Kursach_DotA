using System.Windows.Media;
using System.Windows.Shapes;

namespace DotA.Forms
{
	internal class Circle : Form
	{
		private const int Radius = 5;

		public Circle(Color color) : base(color)
		{
		}

		protected override void Create()
		{
			Obj = new Ellipse
			{
				Stroke = new SolidColorBrush(Color),
				StrokeThickness = 1,
				Fill = new SolidColorBrush(Color),
				Width = 2*Radius,
				Height = 2*Radius,
				RenderTransform = new TranslateTransform(-100, -100)
			};
		}

		public override sealed void SetPosition(int x, int y)
			=> ((Ellipse) Obj).RenderTransform = new TranslateTransform(x - Radius, y - Radius);
	}
}