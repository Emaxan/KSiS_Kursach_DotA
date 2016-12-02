using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using GeneralClasses;

namespace DotA.Properties.Pages
{
	/// <summary>
	///     Логика взаимодействия для Settings.xaml
	/// </summary>
	public partial class Settings
	{
		public EventHandler<MyArgs> BackClick;
		public Main Main;
		private bool _lines = false;

		public Color UserColor;
		public Form UserForm;

		public Settings()
		{
			InitializeComponent();
		}

		private void CMain_OnLoaded(object sender, RoutedEventArgs e)
		{
			TbName.Text = Properties.Settings.Default.UserName;

			var color = Properties.Settings.Default.UserColor;
			if (color == Colors.Red) RbRed.IsChecked = true;
			else if (color == Colors.Blue) RbBlue.IsChecked = true;
			else if (color == Colors.Green) RbGreen.IsChecked = true;
			else if (color == Colors.Yellow) RbYellow.IsChecked = true;
			else
			{
				//Todo Error!!!
			}

			switch (Properties.Settings.Default.UserForm)
			{
				case 0:
					RbCircle.IsChecked = true;
					break;
				case 1:
					RbStar.IsChecked = true;
					break;
				case 2:
					RbTriangle.IsChecked = true;
					break;
				default:
					//Todo Error!!!
					break;
			}

			TbServerIp.Text = Properties.Settings.Default.ServerIp;
			CbLanguage.SelectedIndex = Equals(Properties.Settings.Default.AppCulture, System.Globalization.CultureInfo.GetCultureInfo("ru-RU"))
				? 0
				: 1;

			if (_lines) return;
			Kernel.DrawLine(CMain);
			_lines = true;
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
			Properties.Settings.Default.UserForm = (byte) UserForm;
			Properties.Settings.Default.UserName = TbName.Text;
			Properties.Settings.Default.ServerIp = TbServerIp.Text;
			Properties.Settings.Default.AppCulture = CbLanguage.SelectedIndex == 0
				? System.Globalization.CultureInfo.GetCultureInfo("ru-RU")
				: System.Globalization.CultureInfo.GetCultureInfo("en-EN");
			Properties.Settings.Default.Save();
			LExit_OnMouseLeftButtonUp(sender, e);
		}

		private void RbForm_Checked(object sender, RoutedEventArgs e)
		{
			if (sender.Equals(RbCircle)) UserForm = Form.Circle;
			else if (sender.Equals(RbStar)) UserForm = Form.Star;
			else if (sender.Equals(RbTriangle)) UserForm = Form.Triangle;
		}
	}
}