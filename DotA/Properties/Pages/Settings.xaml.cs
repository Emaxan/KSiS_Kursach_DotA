using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace DotA.Properties.Pages
{
	/// <summary>
	///     Логика взаимодействия для Settings.xaml
	/// </summary>
	public partial class Settings
	{
		

		public EventHandler<MyArgs> BackClick;
		public Main Main;

		public Color UserColor;

		public Settings()
		{
			InitializeComponent();
		}

		private void CMain_OnLoaded(object sender, RoutedEventArgs e)
		{
			TbName.Text = (string) Properties.Settings.Default.UserName;

			var color = (Color) Properties.Settings.Default.UserColor;
			if (color == Colors.Red) RbRed.IsChecked = true;
			else if (color == Colors.Blue) RbBlue.IsChecked = true;
			else if (color == Colors.Green) RbGreen.IsChecked = true;
			else if (color == Colors.Yellow) RbYellow.IsChecked = true;

			TbServerIp.Text = (string) Properties.Settings.Default.ServerIp;
			
			Kernel.DrawLine(CMain);
		}

		private void Label_MouseEnter(object sender, MouseEventArgs e)
		{
			Kernel.MouseEnter(sender,FontSizeProperty);
		}

		private void Label_MouseLeave(object sender, MouseEventArgs e)
		{
			Kernel.MouseLeave(sender, FontSizeProperty);
		}

		private void LExit_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			BackClick?.Invoke(this, new MyArgs
			{
				Root = Main,
				IsUri = false
			});
		}

		private void RadioButton_Checked(object sender, RoutedEventArgs e)
		{
			if (sender.Equals(RbRed)) UserColor = Colors.Red;
			else if (sender.Equals(RbBlue)) UserColor = Colors.Blue;
			else if (sender.Equals(RbGreen)) UserColor = Colors.Green;
			else if (sender.Equals(RbYellow)) UserColor = Colors.Yellow;
		}

		private void LSave_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			Properties.Settings.Default.UserColor = UserColor;
			Properties.Settings.Default.UserName = TbName.Text;
			Properties.Settings.Default.ServerIp = TbServerIp.Text;
			Properties.Settings.Default.Save();
			LExit_OnMouseLeftButtonUp(sender, e);
		}
	}
}