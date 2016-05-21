using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DotA.Properties.Pages;

namespace DotA
{
	/// <summary>
	///     Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow
	{
		private Game _game;
		private Main _main;
		private Settings _settings;
		private GameParams _gameParams;

		public MainWindow()
		{
			Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(Properties.Settings.Default.AppCulture.ToString());
			InitializeComponent();
		}

		private void BExit_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			var b = new ImageBrush
			{
				ImageSource = new BitmapImage(new Uri("img/ButtonOffBG1.png", UriKind.Relative))
			};
			((Border) sender).Background = b;
		}

		private void BExit_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			var b = new ImageBrush
			{
				ImageSource = new BitmapImage(new Uri("img/ButtonOffBG.png", UriKind.Relative))
			};
			((Border) sender).Background = b;
			Application.Current.Shutdown();
		}

		private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
		{
			_main = new Main();
			_settings = new Settings();
			_game = new Game();
			_gameParams = new GameParams();

			_main.GameParams = _gameParams;
			_main.Settings = _settings;
			_main.NewGameClick += ChangePage;
			_main.SettingsClick += ChangePage;
			_main.ExitClick += ExitClick;
			
			_settings.Main = _main;
			_settings.BackClick += ChangePage;
			
			_game.Main = _main;
			_game.ExitClick += ChangePage;

			_gameParams.Game = _game;
			_gameParams.Main = _main;
			_gameParams.ExitClick += ChangePage;
			_gameParams.StartClick += ChangePage;

			FMain.Navigate(_main);
		}

		private static void ExitClick(object sender, EventArgs eventArgs) => Application.Current.Shutdown(0);

		private void ChangePage(object sender, MyArgs myArgs) => FMain.Navigate(myArgs.IsUri ? myArgs.Uri : myArgs.Root);
	}

	public class MyArgs : EventArgs
	{
		public bool IsUri;
		public object Root;
		public Uri Uri;
	}
}