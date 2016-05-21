using System.Windows;
using System.Windows.Media;

namespace DotA.Forms
{
	internal abstract class Form
	{
		protected Form(Color color)
		{
			Color = color;
		}
		private UIElement _obj;
		public UIElement Obj {
			get
			{
				if (_obj == null) Create();
				return _obj;
			}
			protected set { _obj = value; }
		}
		public Color Color;

		protected abstract void Create();
		public abstract void SetPosition(int x, int y);
	}
}