using System;
using System.Windows;
using System.Windows.Input;

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
		private bool _lines = false;

		public Main( ) { InitializeComponent( ); }

		private void CMain_OnLoaded(object sender, RoutedEventArgs e)
		{
			if (_lines) return;
			Kernel.DrawLine(CMain);
			_lines = true;
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
