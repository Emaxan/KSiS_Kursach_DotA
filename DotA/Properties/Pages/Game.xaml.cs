using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using DotA.Forms;
using GeneralClasses;
using Microsoft.AspNet.SignalR.Client;
using static DotA.Kernel;

namespace DotA.Properties.Pages
{
	/// <summary>
	///     Логика взаимодействия для Game.xaml
	/// </summary>
	public partial class Game
	{
		#region Work with form

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
			DrawLine(CMain);
			_gs.X = Properties.Settings.Default.FieldW;
			_gs.Y = Properties.Settings.Default.FieldH;
			User.UserName = Properties.Settings.Default.UserName;
			User.UserColor = Properties.Settings.Default.UserColor;
			User.UserForm = (Form) Properties.Settings.Default.UserForm;
			if (User.UserColor == Colors.Yellow) CbColor.SelectedIndex = 3;
			else if (User.UserColor == Colors.Blue) CbColor.SelectedIndex = 2;
			else if (User.UserColor == Colors.Green) CbColor.SelectedIndex = 1; 
			else if (User.UserColor == Colors.Red) CbColor.SelectedIndex = 0;
			TbName.Text = User.UserName;
			switch (User.UserForm)
			{
				case Form.Circle:
					_dot = new Circle(User.UserColor);
					CbForm.SelectedIndex = 0;
					break;
				case Form.Star:
					_dot = new Star(User.UserColor);
					CbForm.SelectedIndex = 1;
					break;
				case Form.Cross:
					_dot = new Cross(User.UserColor);
					CbForm.SelectedIndex = 2;
					break;
			}
			ConnectAsync();
		}

		private void Game_MouseMove(object sender, MouseEventArgs e)
		{
			var pos = e.MouseDevice.GetPosition(this);
			var xc = ((int) pos.X + LineWidth/2)/LineWidth;
			var yc = ((int) pos.Y + LineWidth/2)/LineWidth;
			var x = xc*LineWidth;
			var y = yc*LineWidth;
			xc -= OffsetX;
			yc -= OffsetY;
			if (!((x >= OffsetX*LineWidth) && (x < (OffsetX + FieldW)*LineWidth) &&
			      (y >= OffsetY*LineWidth) && (y < (OffsetY + FieldH)*LineWidth) &&
			      _matrix[yc, xc] == 255)) return;
			_dot.SetPosition(x, y);
		}

		private async void Game_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (!Connected || !LogIn || !RoomSeted)
			{
				return;
			}

			var pos = e.MouseDevice.GetPosition(this);
			var xc = ((int) pos.X + LineWidth/2)/LineWidth;
			var yc = ((int) pos.Y + LineWidth/2)/LineWidth;
			var x = xc*LineWidth;
			var y = yc*LineWidth;
			xc -= OffsetX;
			yc -= OffsetY;
			if (!((x >= OffsetX*LineWidth) && (x < (OffsetX + FieldW)*LineWidth) &&
			      (y >= OffsetY*LineWidth) && (y < (OffsetY + FieldH)*LineWidth) &&
			      _matrix[yc, xc] == 255))
			{
				MessageBox.Show("You can't do it!cli");
				return;
			}
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
			if (!await HubProxy.Invoke<bool>("SetDotPosition", xc, yc))
			{
				MessageBox.Show("You can't do it!");
				return;
			}
			temp.SetPosition(x, y);
			_matrix[yc, xc] = User.UserColorByte;
			CMain.Children.Add(temp.Obj);
			AnalyzeMatrix();
		}

		public void AnalyzeMatrix()
		{
		}
		
		private async void Start_Click(object sender, RoutedEventArgs e)
		{
			while (!Connected){}

			User.UserName = TbName.Text;
			var color = (byte)CbColor.SelectedIndex;
			User.UserColorByte = color;
			switch (color)
			{
				case 0:
					User.UserColor = Colors.Red;
					_dot.Color = Colors.Red;
					break;
				case 1:
					User.UserColor = Colors.Green;
					_dot.Color = Colors.Green;
					break;
				case 2:
					User.UserColor = Colors.Blue;
					_dot.Color = Colors.Blue;
					break;
				case 3:
					User.UserColor = Colors.Yellow;
					_dot.Color = Colors.Yellow;
					break;
			}
			var form = (byte) CbForm.SelectedIndex;
			switch (form)
			{
				case 0:
					User.UserForm = Form.Circle;
					break;
				case 1:
					User.UserForm = Form.Star;
					break;
				case 2:
					User.UserForm = Form.Cross;
					break;
			}
			if (!RoomSeted)
			{
				if ((LvRooms.SelectedIndex == -1) || (LvRooms.Items[0] is ListViewItem))
				{
					MessageBox.Show("Wrong Room!!");
					return;
				}
			}
			if (!LogIn)
			{
				await HubProxy.Invoke("join", User.UserName, color, form);
				while (!LogIn) { }
			}
			if (!RoomSeted)
			{
				var res = await HubProxy.Invoke<bool>("setRoom", ((ChatRoom)LvRooms.SelectedItem).Name);
				if (res) while (!RoomSeted) { }
				else
				{
					MessageBox.Show("Cant Set Room");
					return;
				}
			}

			BForm.Visibility = Visibility.Collapsed;
			CMain.Children.Clear();
			CMain.Children.Add(_dot.Obj);
			DrawLine(CMain, _gs.Y, _gs.X);
			_matrix = new byte[FieldH + 1, FieldW + 1];
			for (var i = 0; i < FieldH + 1; i++)
			{
				for (var j = 0; j < FieldW + 1; j++)
				{
					_matrix[i, j] = 255;
				}
			}
		}

		private void AddRoom_Click (object sender, RoutedEventArgs e)
		{
			TbRoomName.Text = string.Empty;
			BForm.Visibility = Visibility.Collapsed;
			BRoomName.Visibility = Visibility.Visible;
		}
		
		private async void BOk_OnClick(object sender, RoutedEventArgs e)
		{
			var key = TbRoomName.Text;
			if (await HubProxy.Invoke<bool>("AddRoom", key))
			{
				User.UserName = TbName.Text;
				var color = (byte)CbColor.SelectedIndex;
				User.UserColorByte = color;
				switch (color)
				{
					case 0:
						User.UserColor = Colors.Red;
						break;
					case 1:
						User.UserColor = Colors.Green;
						break;
					case 2:
						User.UserColor = Colors.Blue;
						break;
					case 3:
						User.UserColor = Colors.Yellow;
						break;
				}
				var form = (byte)CbForm.SelectedIndex;
				switch (form)
				{
					case 0:
						User.UserForm = Form.Circle;
						break;
					case 1:
						User.UserForm = Form.Star;
						break;
					case 2:
						User.UserForm = Form.Cross;
						break;
				}
				await HubProxy.Invoke("join", User.UserName, User.UserColorByte, (byte)User.UserForm);
				while (!LogIn) { }
				if (await HubProxy.Invoke<bool>("SetRoom", key))
				{
					await HubProxy.Invoke("SetRoomField", _gs.X, _gs.Y);
					Rescan_Click(sender, e);
				}
			}
			if (!RoomSeted)
			{
				BRoomName.Visibility = Visibility.Collapsed;
				BForm.Visibility = Visibility.Visible;
			}
			else
				Start_Click(sender, e);
		}

		private void BCancel_OnClick(object sender, RoutedEventArgs e)
		{
			BRoomName.Visibility = Visibility.Collapsed;
			BForm.Visibility = Visibility.Visible;
		}

		private async void DeleteRoom_Click(object sender, RoutedEventArgs e)
		{
			if (LvRooms.SelectedIndex < 0) return;
			var key = ((ChatRoom) LvRooms.SelectedItem).Name;
			var res = await HubProxy.Invoke<bool>("DeleteRoom", key);
			if (res) Rescan_Click(sender, e);
		}

		private async void Rescan_Click(object sender, RoutedEventArgs e)
		{
			var rooms = await HubProxy.Invoke<IEnumerable<ChatRoom>>("GetRooms");
			LvRooms.ItemsSource = null;
			LvRooms.Items.Clear();
			var source = rooms as IList<ChatRoom> ?? rooms.ToList();
			if (source.Count != 0)
				LvRooms.ItemsSource = source;
			else
				LvRooms.Items.Add(new ListViewItem { Content = "No opened rooms" });
		}

		#endregion

		#region Connection To Server

		public bool Connected, LogIn, RoomAdded, RoomSeted;
		public IHubProxy HubProxy { get; set; }
		public HubConnection Connection { get; set; }


		public const string ServerUri = "http://localhost:13666/signalr";

		private async void ConnectAsync()
		{
			Connection = new HubConnection(ServerUri);
			Connection.Closed += Connection_Closed;
			Connection.Error += ex => Console.WriteLine($"SignalR error: {ex.Message}");
			Connection.TraceLevel = TraceLevels.All;
			Connection.TraceWriter = Console.Out;
			ServicePointManager.DefaultConnectionLimit = 10;
			HubProxy = Connection.CreateHubProxy("MyHub");

			HubProxy.On<string, string>("AddMessage", (name, message) => Dispatcher.Invoke(() => { MessageBox.Show($"{name}: {message}\r"); }));
			HubProxy.On<string>("AddMessage", message => Dispatcher.Invoke(() => { MessageBox.Show($"{message}\r"); }));
			HubProxy.On("ConnectionComplete", () => Connected = true);
			HubProxy.On("UserLoggedIn", () => LogIn = true);
			HubProxy.On("RoomAdded", () => RoomAdded = true);
			HubProxy.On("RoomSeted", () => RoomSeted = true);
			HubProxy.On<byte, byte, byte, byte>("SetDot", (x, y, color, form) =>
			{
				x += (byte)OffsetX;
				y += (byte)OffsetY;
				//if (Connection.ConnectionId == id) return;
				Forms.Form temp;
				Color Ucolor;
				switch (color)
				{
					case 0:
						Ucolor = Colors.Red;
						break;
					case 1:
						Ucolor = Colors.Green;
						break;
					case 2:
						Ucolor = Colors.Blue;
						break;
					case 3:
						Ucolor = Colors.Yellow;
						break;
					default:
						Ucolor = User.UserColor;
						break;
				}
				switch (form)
				{
					case 0:
						temp = new Circle(Ucolor);
						break;
					case 1:
						temp = new Star(Ucolor);
						break;
					case 2:
						temp = new Cross(Ucolor);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
				Application.Current.Dispatcher.Invoke(() => temp.SetPosition(x*LineWidth, y*LineWidth));
				_matrix[y, x] = color;
				Application.Current.Dispatcher.Invoke(() => CMain.Children.Add(temp.Obj));
			});
			try
			{
				await Connection.Start();
			}
			catch (HttpRequestException)
			{
				MessageBox.Show("Unable to connect to server.");
			}
			
			var rooms = await HubProxy.Invoke<IEnumerable<ChatRoom>>("GetRooms");
			var source = rooms as IList<ChatRoom> ?? rooms.ToList();
			if (source.Count != 0)
				LvRooms.ItemsSource = source;
			else
				LvRooms.Items.Add(new ListViewItem {Content = "No opened rooms"});
		}

		private void Connection_Closed()
		{
			//TODO
		}

		#endregion
	}

	public class User
	{
		public string UserName { get; set; }
		public Color UserColor { get; set; }
		public byte UserColorByte { get; set; }
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