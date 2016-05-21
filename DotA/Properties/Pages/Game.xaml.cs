using System;
using System.Net.Http;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using DotA.Forms;
using Microsoft.AspNet.SignalR.Client;
using static DotA.Kernel;

namespace DotA.Properties.Pages
{
	/// <summary>
	///     Логика взаимодействия для Game.xaml
	/// </summary>
	public partial class Game
	{
		private readonly GameSettings _gs = new GameSettings();
		private Forms.Form _dot;
		private byte[,] _matrix;
		public EventHandler<MyArgs> ExitClick;
		public Main Main;
		public User User = new User();

		public Game()
		{
			InitializeComponent();
		}

		private void CMain_OnLoaded(object sender, RoutedEventArgs e)
		{
			_gs.X = Properties.Settings.Default.FieldW;
			_gs.Y = Properties.Settings.Default.FieldH;
			User.UserName = Properties.Settings.Default.UserName;
			User.UserColor = Properties.Settings.Default.UserColor;
			User.UserForm = (Form)Properties.Settings.Default.UserForm;
			switch (User.UserForm)
			{
				case Form.Circle:
					_dot = new Circle(User.UserColor);
					break;
				case Form.Star:
					_dot = new Star(User.UserColor);
					break;
				case Form.Cross:
					_dot = new Cross(User.UserColor);
					break;
			}
			CMain.Children.Add(_dot.Obj);
			DrawLine(CMain, _gs.Y, _gs.X);
			_matrix = new byte[FieldH + 1, FieldW + 1];
			ConnectAsync();
		}

		private void Game_MouseMove(object sender, MouseEventArgs e)
		{
			var pos = e.MouseDevice.GetPosition(this);
			var xc = ((int)pos.X + LineWidth / 2) / LineWidth;
			var yc = ((int)pos.Y + LineWidth / 2) / LineWidth;
			var x = xc * LineWidth;
			var y = yc * LineWidth;
			xc -= OffsetX;
			yc -= OffsetY;
			if (!((x >= OffsetX * LineWidth) && (x < (OffsetX + FieldW) * LineWidth) &&
				  (y >= OffsetY * LineWidth) && (y < (OffsetY + FieldH) * LineWidth) &&
				  _matrix[yc, xc] == 0)) return;
			_dot.SetPosition(x, y);
		}

		private void Game_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			HubProxy.Invoke("Show", $"{User.UserName}");
			HubProxy.Invoke("SendClients");
			var pos = e.MouseDevice.GetPosition(this);
			var xc = ((int) pos.X + LineWidth/2)/LineWidth;
			var yc = ((int) pos.Y + LineWidth/2)/LineWidth;
			var x = xc*LineWidth;
			var y = yc*LineWidth;
			xc -= OffsetX;
			yc -= OffsetY;
			if (!((x >= OffsetX*LineWidth) && (x < (OffsetX + FieldW)*LineWidth) &&
			      (y >= OffsetY*LineWidth) && (y < (OffsetY + FieldH)*LineWidth) &&
			      _matrix[yc, xc] == 0)) return;
			_dot.SetPosition(x, y);
			Forms.Form temp;
			switch (User.UserForm)
			{
				case Form.Circle:
					temp = new Circle(User.UserColor);
					break;
				case Form.Star:
					temp = new Star(User.UserColor);
					break;
				case Form.Cross:
					temp = new Cross(User.UserColor);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			temp.SetPosition(x, y);
			_matrix[yc, xc] = 1;
			CMain.Children.Add(temp.Obj);
			AnalyzeMatrix();
		}

		public void AnalyzeMatrix()
		{
			
		}

		#region Connection To Server

		public string UserName { get; set; }
		public IHubProxy HubProxy { get; set; }
		public HubConnection Connection { get; set; }
		public const string ServerUri = "http://localhost:13666/signalr";
		public int Room;
		private async void ConnectAsync()
		{
			Connection = new HubConnection(ServerUri);
			Connection.Closed += Connection_Closed;
			HubProxy = Connection.CreateHubProxy(User.UserName);

			HubProxy.On<string, string, int>("AddMessage", (name, message, room) =>
				Dispatcher.Invoke(() =>
				{ if (Room == room) MessageBox.Show($"{name}: {message}\r"); }
					)
				);
			HubProxy.On<string, int>("AddString", (message, room) =>
				 Dispatcher.Invoke(() =>
				 { if (Room == room) MessageBox.Show($"{message}\r"); }
				 )
			 );
			try
			{
				await Connection.Start();
			}
			catch (HttpRequestException)
			{
				MessageBox.Show("Unable to connect to server: Start server before connecting clients.");
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message);
			}
		}

		void Connection_Closed()
		{
		}

		#endregion
	}

	public class User
	{
		public string UserName { get; set; }
		public Color UserColor { get; set; }
		public Form UserForm { get; set; }
	}

	public class GameSettings
	{
		public int X { get; set; }
		public int Y { get; set; }
	}

	public enum Form
	{
		Circle = 0,
		Star = 1,
		Cross = 2
	}
}