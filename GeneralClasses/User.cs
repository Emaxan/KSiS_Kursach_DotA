using System.Windows.Media;

namespace GeneralClasses
{
	public class User
	{
		public string UserName { get; set; }

		private Color _userColor;
		public Color UserColor
		{
			get { return _userColor; }
			set
			{
				_userColor = value;
				if (value == Colors.Yellow) UserColorByte = 3;
				else if (value == Colors.Blue) UserColorByte = 2;
				else if (value == Colors.Green) UserColorByte = 1;
				else if (value == Colors.Red) UserColorByte = 0;
			}
		}
		public byte UserColorByte { get; private set; }
		public Form UserForm { get; set; }
	}

	public enum Form
	{
		Circle = 0,
		Star = 1,
		Triangle = 2
	}
}