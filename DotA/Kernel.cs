using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace DotA
{
	public static class Kernel
	{
		public static int OffsetX, OffsetY, FieldH, FieldW;

		public static int LineWidth = 18;

		public static void DrawLine(Canvas cMain)
		{
			for (var i = 0; (i < cMain.ActualWidth)||(i < cMain.ActualHeight); i += LineWidth)
			{
				if (i < cMain.ActualWidth)
				{
					var li = new Line
					{
						Stroke = Brushes.Black,
						StrokeThickness = 0.5,
						X1 = i,
						Y1 = 0,
						X2 = i,
						Y2 = cMain.ActualHeight
					};
					cMain.Children.Add(li);
				}
				if (i < cMain.ActualHeight)
				{
					var li = new Line
					{
						Stroke = Brushes.Black,
						StrokeThickness = 0.5,
						X1 = 0,
						Y1 = i,
						X2 = cMain.ActualWidth,
						Y2 = i
					};
					cMain.Children.Add(li);
				}
			}
		}

		public static void DrawLine(Canvas cMain, int fieldH, int fieldW)
		{
			FieldH = fieldH;
			FieldW = fieldW;
			var realH = (int)(cMain.ActualHeight/LineWidth);
			var realW = (int)(cMain.ActualWidth/LineWidth);
			OffsetX = (realW - fieldW)/2 + (realW - fieldW)%2;
			OffsetY = (realH - fieldH)/2 + (realH - fieldH)%2;
			for (var i = OffsetX*LineWidth; i < (fieldW+OffsetX)*LineWidth; i += LineWidth)//vertic
			{
				var li = new Line
				{
					Stroke = Brushes.Black,
					StrokeThickness = 0.5,
					X1 = i,
					Y1 = OffsetY*LineWidth,
					X2 = i,
					Y2 = (fieldH + OffsetY - 1)*LineWidth
				};
				cMain.Children.Add(li);
			}
			for (var i = OffsetY*LineWidth; i < (fieldH+OffsetY)*LineWidth; i += LineWidth)//horiz
			{
				var li = new Line
				{
					Stroke = Brushes.Black,
					StrokeThickness = 0.5,
					X1 = OffsetX*LineWidth,
					Y1 = i,
					X2 = (fieldW + OffsetX - 1)*LineWidth,
					Y2 = i
				};
				cMain.Children.Add(li);
			}
		}

		public static void MouseEnter(object sender, DependencyProperty property)
		{
			var a = new DoubleAnimation
			{
				From = 50,
				To = 60,
				Duration = new Duration(TimeSpan.FromMilliseconds(100))
			};
			((Label)sender).BeginAnimation(property, a);
		}

		public static void MouseLeave(object sender, DependencyProperty property)
		{
			var a = new DoubleAnimation
			{
				From = 60,
				To = 50,
				Duration = new Duration(TimeSpan.FromMilliseconds(100))
			};
			((Label)sender).BeginAnimation(property, a);
		}
	}
}