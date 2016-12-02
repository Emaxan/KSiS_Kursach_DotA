using System.Windows.Media;

namespace GeneralClasses
{
	public class Dot
	{
		public Dot(int x, int y, Color color)
		{
			X = x;
			Y = y;
			Color = color;
		}

		public int X;
		public int Y;

		private Color _color;
		public Color Color
		{
			get { return _color; }
			set
			{
				_color = value;
				if (value == Colors.Yellow) ColorByte = 3;
				else if (value == Colors.Blue) ColorByte = 2;
				else if (value == Colors.Green) ColorByte = 1;
				else if (value == Colors.Red) ColorByte = 0;
			}
		}
		public byte ColorByte { get; private set; }
	}
}