using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using DotA.Forms;
using GeneralClasses;
using Microsoft.AspNet.SignalR.Client;
using static DotA.Kernel;
using Form = GeneralClasses.Form;

namespace DotA.Properties.Pages//TODO When user quit game change turn queue
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
		private const byte MatrixDefolt = 255;
		private bool[,] _boolMatrix;
		public EventHandler<MyArgs> ExitClick;
		public Main Main;
		public User User = new User();
		public byte SelectedUserForm = 255;

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
				case Form.Triangle:
					_dot = new Triangle(User.UserColor);
					CbForm.SelectedIndex = 2;
					break;
				default:
					//Todo Error!!!
					break;
			}
			TbConnectionIp.Text = Properties.Settings.Default.ServerIp;
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
			      _matrix[yc, xc] == MatrixDefolt)) return;
			_dot.SetPosition(x, y);
		}

		private async void Game_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (!Connected || !LogIn || !RoomSeted || !YourTurn)
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
			      _matrix[yc, xc] == MatrixDefolt))
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
				case Form.Triangle:
					temp = new Triangle(User.UserColor);
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
			YourTurn = false;
			var list = new List<Dot>();
			if (AnalyzeMatrix(xc, yc, ref list))
			{
				//TODO send signals
				await HubProxy.Invoke("SendField", list);
			}
		}

		public bool AnalyzeMatrix(int x, int y, ref List<Dot> path)
		{
			var list = new Stack<Dot>();
			if (!TryCloseField(x, y, ref list)) return false;
			if (!CloseField(list)) return false;
			path = list.ToList();
			return true;
		}

		private bool TryCloseField(int x, int y, ref Stack<Dot> stack)
		{
			if (stack.Count > 1)
			{
				var last = stack.Pop();
				var prev = stack.Peek();
				stack.Push(last);
				if ((prev.X == x) && (prev.Y == y)) return false;
			}
			if (_boolMatrix[y, x]) return true;
			_boolMatrix[y, x] = true;
			stack.Push(new Dot(x, y, User.UserColor, User.UserForm));
			var res = false;

			if ((y > 0) && (x > 0) && (_matrix[y - 1, x - 1] == User.UserColorByte))
			{
				res = TryCloseField(x - 1, y - 1, ref stack);
			}
			if (!res && (y > 0) && (_matrix[y - 1, x] == User.UserColorByte))
			{
				res = TryCloseField(x, y - 1, ref stack);
			}
			if (!res && (y > 0) && (x < FieldW) && (_matrix[y - 1, x + 1] == User.UserColorByte))
			{
				res = TryCloseField(x + 1, y - 1, ref stack);
			}
			if (!res && (x > 0) && (_matrix[y, x - 1] == User.UserColorByte))
			{
				res = TryCloseField(x - 1, y, ref stack);
			}
			if (!res && (x < FieldW) && (_matrix[y, x + 1] == User.UserColorByte))
			{
				res = TryCloseField(x + 1, y, ref stack);
			}
			if (!res && (x > 0) && (y < FieldH) && (_matrix[y + 1, x - 1] == User.UserColorByte))
			{
				res = TryCloseField(x - 1, y + 1, ref stack);
			}
			if (!res && (y < FieldH) && (_matrix[y + 1, x] == User.UserColorByte))
			{
				res = TryCloseField(x, y + 1, ref stack);
			}
			if (!res && (x < FieldW) && (y < FieldH) && (_matrix[y + 1, x + 1] == User.UserColorByte))
			{
				res = TryCloseField(x + 1, y + 1, ref stack);
			}
			if (!res)
			{
				_boolMatrix[y, x] = false;
				stack.Pop();
			}
			return res;
		}

		private bool CloseField(IEnumerable<Dot> list)
		{
			var dots = list.ToArray();
			if ((list == null) || (!dots.Any())) return false;
			var col = new PointCollection();
			foreach (var dot in dots)
			{
				col.Add(new Point((dot.X + OffsetX) * LineWidth, (dot.Y + OffsetY)* LineWidth));
			}
			var bg = dots[0].Color;
			bg.A = 50;
			var stroke = dots[0].Color;
			stroke.A = 150;
			CMain.Children.Add(new Polygon
			{
				Points = col,
				Stroke = new SolidColorBrush(stroke),
				StrokeThickness = 3,
				Fill = new SolidColorBrush(bg)
			});
			return true;
		}

		private async void Start_Click(object sender, RoutedEventArgs e)
		{
			while (!Connected){}

			User.UserName = TbName.Text;
			switch (CbColor.SelectedIndex)
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
				default:
					//Todo Error!!!
					break;
			}
			User.UserForm = (Form) CbForm.SelectedIndex;
			switch (User.UserForm)
			{
				case Form.Circle:
					_dot = new Circle(User.UserColor);
					break;
				case Form.Star:
					_dot = new Star(User.UserColor);
					break;
				case Form.Triangle:
					_dot = new Triangle(User.UserColor);
					break;
				default:
					//Todo Error!!!
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
				await HubProxy.Invoke("join", User);
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
			_boolMatrix = new bool[FieldH + 1, FieldW + 1];
			for (var i = 0; i < FieldH + 1; i++)
			{
				for (var j = 0; j < FieldW + 1; j++)
				{
					_matrix[i, j] = MatrixDefolt;
					_boolMatrix[i, j] = false;
				}
			}
			CloseField(new List<Dot>
			{
				new Dot(1, 1, User.UserColor, User.UserForm),
				new Dot(2, 2, User.UserColor, User.UserForm),
				new Dot(3, 1, User.UserColor, User.UserForm),
				new Dot(2, 0, User.UserColor, User.UserForm)
			});
		}

		private void AddRoom_Click (object sender, RoutedEventArgs e)
		{
			if(!Connected) return;
			TbRoomName.Text = string.Empty;
			BForm.Visibility = Visibility.Collapsed;
			BRoomName.Visibility = Visibility.Visible;
		}
		
		private async void BOk_OnClick(object sender, RoutedEventArgs e)
		{
			if(!Connected) return;
			var key = TbRoomName.Text;
			if (await HubProxy.Invoke<bool>("AddRoom", key))
			{
				User.UserName = TbName.Text;
				switch (CbColor.SelectedIndex)
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
					default:
						//Todo Error!!!
						break;
				}
				User.UserForm = (Form) CbForm.SelectedIndex;
				await HubProxy.Invoke("join", User);
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
			if (!Connected) return;
			if (LvRooms.SelectedIndex < 0) return;
			var key = ((ChatRoom) LvRooms.SelectedItem).Name;
			var res = await HubProxy.Invoke<bool>("DeleteRoom", key);
			if (res) Rescan_Click(sender, e);
		}

		private async void Rescan_Click(object sender, RoutedEventArgs e)
		{
			if(!Connected) return;
			var rooms = await HubProxy.Invoke<IEnumerable<ChatRoom>>("GetRooms");
			LvRooms.ItemsSource = null;
			LvRooms.Items.Clear();
			var source = rooms as IList<ChatRoom> ?? rooms.ToList();
			if (source.Count != 0)
				LvRooms.ItemsSource = source;
			else
				LvRooms.Items.Add(new ListViewItem { Content = Properties.Resources.Resource_main.NoOpenedRooms });
		}

		private void BOk_OnClickConection(object sender, RoutedEventArgs e)
		{
			ServerUri = $"http://{TbConnectionIp.Text}:13666/signalr";
			ConnectAsync();
			BConnectionAddres.Visibility = Visibility.Collapsed;
			BForm.Visibility = Visibility.Visible;
		}

		private void BCancelConnection_OnClick(object sender, RoutedEventArgs e)
		{
			ExitClick?.Invoke(sender,new MyArgs() {IsUri = false,Root = Main});
		}

		private void ActivateButtons()
		{
			BStart.IsEnabled = true;
			BAddRoom.IsEnabled = true;
			BRescan.IsEnabled = true;
			BDeleteRoom.IsEnabled = true;
		}

		#endregion

		#region Connection To Server

		public bool Connected, LogIn, RoomAdded, RoomSeted, YourTurn;
		public IHubProxy HubProxy { get; set; }
		public HubConnection Connection { get; set; }


		public string ServerUri;

		private async void ConnectAsync()
		{
			Connection = new HubConnection(ServerUri);
			//Connection.Closed += Connection_Closed;
			Connection.Error += ex => Console.WriteLine($"SignalR error: {ex.Message}");
			Connection.TraceLevel = TraceLevels.All;
			Connection.TraceWriter = Console.Out;
			ServicePointManager.DefaultConnectionLimit = 10;
			HubProxy = Connection.CreateHubProxy("MyHub");

			HubProxy.On<string, string>("AddMessage", (name, message) => 
																Dispatcher.Invoke(() => 
																				MessageBox.Show($"{name}: {message}\r")
																				)
										);
			HubProxy.On<string>("AddMessage", message => Dispatcher.Invoke(() => MessageBox.Show($"{message}\r")));
			HubProxy.On("ConnectionComplete", () => Connected = true);
			HubProxy.On("UserLoggedIn", () => LogIn = true);
			HubProxy.On("RoomAdded", () => RoomAdded = true);
			HubProxy.On("RoomSeted", () => RoomSeted = true);
			HubProxy.On<byte, byte, byte, byte>("SetDot", (x, y, color, form) =>
			{
				Forms.Form temp;
				Color ucolor;
				switch (color)
				{
					case 0:
						ucolor = Colors.Red;
						break;
					case 1:
						ucolor = Colors.Green;
						break;
					case 2:
						ucolor = Colors.Blue;
						break;
					case 3:
						ucolor = Colors.Yellow;
						break;
					default:
						ucolor = User.UserColor;
						break;
				}
				switch (form)
				{
					case 0:
						temp = new Circle(ucolor);
						break;
					case 1:
						temp = new Star(ucolor);
						break;
					case 2:
						temp = new Triangle(ucolor);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
				Application.Current.Dispatcher.Invoke(() => temp.SetPosition((x+OffsetX)*LineWidth, (y+OffsetY)*LineWidth));
				_matrix[y, x] = color;
				Application.Current.Dispatcher.Invoke(() => CMain.Children.Add(temp.Obj));
			});
			HubProxy.On("YourTurn", () => YourTurn = true);
			HubProxy.On<List<Dot>>("CloseField", list => Application.Current.Dispatcher.Invoke(() => CloseField(list)));
			try
			{
				await Connection.Start();
			}
			catch (HttpRequestException)
			{
				MessageBox.Show("Unable to connect to server.");
				BConnectionAddres.Visibility = Visibility.Visible;
				BForm.Visibility = Visibility.Collapsed;
				return;
			}

			ActivateButtons();

			var rooms = await HubProxy.Invoke<IEnumerable<ChatRoom>>("GetRooms");
			var source = rooms as IList<ChatRoom> ?? rooms.ToList();
			if (source.Count != 0)
				LvRooms.ItemsSource = source;
			else
				LvRooms.Items.Add(new ListViewItem {Content = Properties.Resources.Resource_main.NoOpenedRooms });
		}
		#endregion
	}
}