using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace DotA.Properties.Pages {
	/// <summary>
	///     Логика взаимодействия для Main.xaml
	/// </summary>
	public partial class Main {
		public EventHandler ExitClick;
		public EventHandler<MyArgs> NewGameClick;
		public EventHandler<MyArgs> SettingsClick;
		public GameParams GameParams;
		public Settings Settings;

		public Main( ) { InitializeComponent( ); }

		private void CMain_OnLoaded(object sender, RoutedEventArgs e)
		{
			Kernel.DrawLine(CMain);
		}

		private void Label_MouseEnter(object sender, MouseEventArgs e)
		{
			Kernel.MouseEnter(sender, FontSizeProperty);
		}

		private void Label_MouseLeave(object sender, MouseEventArgs e)
		{
			Kernel.MouseLeave(sender, FontSizeProperty);
		}

		private void NewGame_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			NewGameClick?.Invoke(this, new MyArgs {
													Root = GameParams,
													IsUri = false
												});
		}

		private void Settings_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			SettingsClick?.Invoke(this, new MyArgs {
														Root = Settings,
														IsUri = false
													});
		}

		private void Exit_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			ExitClick?.Invoke(this, new EventArgs( ));
		}
	}

}
