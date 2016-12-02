using System;
using System.Windows;
using System.Windows.Input;

namespace DotA.Properties.Pages
{
	/// <summary>
	///     Interaction logic for GameParams.xaml
	/// </summary>
	public partial class GameParams
	{
		public EventHandler<MyArgs> ExitClick;
		public EventHandler<MyArgs> StartClick;
		public Game Game;
		public Main Main;
		private bool _lines = false;

		public GameParams()
		{
			InitializeComponent();
		}

		private void CMain_OnLoaded(object sender, RoutedEventArgs e)
		{
			if(_lines) return;
			Kernel.DrawLine(CMain);
			_lines = true;	
		}

		private void Label_MouseLeave(object sender, MouseEventArgs e)
		{
			Kernel.MouseLeave(sender, FontSizeProperty);
		}

		private void Label_MouseEnter(object sender, MouseEventArgs e)
		{
			Kernel.MouseEnter(sender, FontSizeProperty);
		}

		private void StartClickHandler(object sender, MouseButtonEventArgs e)
		{
			StartClick?.Invoke(sender, new MyArgs
												{
													IsUri = false,
													Root = Game
												});
		}

		private void BackClickHandler(object sender, MouseButtonEventArgs e)
		{
			ExitClick?.Invoke(sender, new MyArgs
												{
													IsUri = false,
													Root = Main
												});
		}
	}
}