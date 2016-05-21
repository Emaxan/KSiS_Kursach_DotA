using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

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

		public GameParams()
		{
			InitializeComponent();
		}

		private void CMain_OnLoaded(object sender, RoutedEventArgs e)
		{
			Kernel.DrawLine(CMain);
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
			StartClick?.Invoke(sender, new MyArgs
			{
				IsUri = false,
				Root = Main
			});
		}
	}
}