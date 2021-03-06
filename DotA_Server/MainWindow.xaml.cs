﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using GeneralClasses;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Hosting;
using Owin;

namespace DotA_Server
{
	/// <summary>
	///     Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow
	{
		private const string ServerUri = "http://*:13666";
		private StreamWriter _logger;
		private string _logPath;
		private bool? _started = false;
		private Timer _t;
		private const int TimerElapse = 10*60*1000; //10 minutes
		public IDisposable SignalR { get; set; }


		public MainWindow()
		{
			InitializeComponent();
		}


		private void Start_OnClick(object sender, RoutedEventArgs e)
		{
			_logPath = Assembly.GetExecutingAssembly().Location;
			_logPath = _logPath?.Substring(0, _logPath.LastIndexOf('\\'));
			_logPath += $"\\log\\{DateTime.Today.ToShortDateString()}_{DateTime.Now.ToShortTimeString().Replace(':', '.')}.txt";
			_logger = new StreamWriter(_logPath, true);
			BStop.IsEnabled = true;
			WriteToConsole("Starting server...");
			BStart.IsEnabled = false;
			Task.Run(() => StartServer());
		}

		private void StartServer()
		{
			try
			{
				SignalR = WebApp.Start(ServerUri);
			}
			catch (TargetInvocationException e)
			{
				WriteToConsole("A server is already running at " + ServerUri + ' ' + e.Message + ' ' + e.InnerException);
				Dispatcher.Invoke(() =>
				{
					BStart.IsEnabled = true;
					BStop.IsEnabled = false;
				});
				_started = null;
				return;
			}
			Dispatcher.Invoke(() => BStop.IsEnabled = true);
			WriteToConsole("Server started at " + ServerUri);
			_started = true;
			_t = new Timer(TimerElapse) { AutoReset = true };
			_t.Elapsed += (o, args) =>
			{
				WriteToConsole("Flush.");
				_logger.Flush();
				_logger.Close();
				_logger = new StreamWriter(_logPath, true);
			};
			_t.Start();
		}

		private void Stop_OnClick(object sender, RoutedEventArgs e)
		{
			BStop.IsEnabled = false;
			BStart.IsEnabled = true;
			SignalR.Dispose();
			WriteToConsole("Server stoped...");
			_logger.Flush();
			_logger.Close();
			_started = false;
			_t.Stop();
		}

		public void WriteToConsole(string message)
		{
			if (!(TbMain.CheckAccess()))
			{
				Dispatcher.Invoke(() =>
					WriteToConsole(message)
					);
				return;
			}
			TbMain.AppendText(message + "\r");
			_logger.Write($"{DateTime.Now}: {message}\r\n");
		}

		private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
		{
			Start_OnClick(sender, e);
		}

		private void MainWindow_OnClosing(object sender, CancelEventArgs e)
		{
			if (BStop.IsEnabled)
				Stop_OnClick(sender, new RoutedEventArgs());
			if (_started == false) return;
			_logger.Flush();
			_logger.Close();
		}
	}

	public class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			var hubConfiguration = new HubConfiguration {EnableDetailedErrors = true};
			app.UseCors(CorsOptions.AllowAll);
			app.MapSignalR(hubConfiguration);
		}
	}

	public class MyHub : Hub
	{
		private static readonly ConcurrentDictionary<string, ChatUser> Users =
			new ConcurrentDictionary<string, ChatUser>(StringComparer.OrdinalIgnoreCase);

		private static readonly ConcurrentDictionary<string, HashSet<string>> UserRooms =
			new ConcurrentDictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);

		private static readonly ConcurrentDictionary<string, ChatRoom> Rooms =
			new ConcurrentDictionary<string, ChatRoom>(StringComparer.OrdinalIgnoreCase);

		private const byte MatrixDefault = 255;

		public void Join(User user)
		{
			AddUser(user.UserName, user.UserColorByte, (byte)user.UserForm);
			Clients.Caller.UserLoggedIn();
			Application.Current.Dispatcher.Invoke(() =>
				((MainWindow) Application.Current.MainWindow).WriteToConsole($"User {user.UserName} has id {Context.ConnectionId}"));
		}

		public void SendMessage(string name, string message)
		{
			Clients.All.addMessage(name, message);
			Application.Current.Dispatcher.Invoke(() =>
				((MainWindow) Application.Current.MainWindow).WriteToConsole($"{Clients.Caller.room}Server send: {name}:{message}"));
		}

		public void SendString(string str)
		{
			Clients.All.addString(str);
			Application.Current.Dispatcher.Invoke(() =>
				((MainWindow) Application.Current.MainWindow).WriteToConsole($"{Clients.Caller.room}: Server send: {str}"));
		}

		public bool SetRoom(string key)
		{
			ChatRoom cr;
			var room = Rooms.TryGetValue(key, out cr);
			if (!room) return false;
			Clients.Caller.room = key;
			Rooms[key].Users.Add(Context.ConnectionId);
			Clients.Caller.RoomSeted();
			UserRooms[Context.ConnectionId].Add(key);
			Rooms[Clients.Caller.room].Queue.Enqueue(Context.ConnectionId);
			if (GetUsers().Count() == 2)
			{
				var queue = (string)Rooms[Clients.Caller.room].Queue.Dequeue();
				Clients.Client(queue).YourTurn();
				Rooms[Clients.Caller.room].Queue.Enqueue(queue);
			}
			Application.Current.Dispatcher.Invoke(() =>
				((MainWindow) Application.Current.MainWindow).WriteToConsole($"User {Clients.Caller.name} insert to the {key} room."));
			return true;
		}

		public bool AddRoom(string key)
		{
			var res = Rooms.TryAdd(key, new ChatRoom {Name = key});
			Clients.Caller.RoomAdded();
			if (!res) return false;
			Rooms[key].Users.Add(Context.ConnectionId);
			Rooms[key].Queue = new Queue<string>();
			Application.Current.Dispatcher.Invoke(() =>
				((MainWindow) Application.Current.MainWindow).WriteToConsole($"Room {key} added."));
			return true;
		}

		public bool DeleteRoom(string key)
		{
			ChatRoom room;
			var res = Rooms.TryRemove(key, out room);
			if (!res) return false;
			Application.Current.Dispatcher.Invoke(() =>
				((MainWindow) Application.Current.MainWindow).WriteToConsole($"Room {key} deleted."));
			return true;
		}

		public override Task OnConnected()
		{
			Application.Current.Dispatcher.Invoke(() =>
				((MainWindow) Application.Current.MainWindow).WriteToConsole("Client connected: " + Context.ConnectionId));
			Clients.Caller.ConnectionComplete();
			return base.OnConnected();
		}

		public override Task OnDisconnected(bool stopCalled)
		{
			Application.Current.Dispatcher.Invoke(() =>
				((MainWindow) Application.Current.MainWindow).WriteToConsole("Client disconnected: " + Context.ConnectionId));

			var user = Users.Values.FirstOrDefault(u => u.ConnectionId == Context.ConnectionId);
			if (user == null) return base.OnDisconnected(stopCalled);

			ChatUser ignoredUser;
			Users.TryRemove(user.Name, out ignoredUser);

			// Leave all rooms
			HashSet<string> rooms;
			if (UserRooms.TryGetValue(user.Name, out rooms))
			{
				foreach (var room in rooms)
				{
					Clients.Group(room).leave(user);
					var chatRoom = Rooms[room];
					chatRoom.Users.Remove(user.Name);
				}
			}

			HashSet<string> ignoredRoom;
			UserRooms.TryRemove(user.Name, out ignoredRoom);
			return base.OnDisconnected(stopCalled);
		}

		public IEnumerable<ChatUser> GetUsers() =>
			GetUsersByRoom(Clients.Caller.room);

		public IEnumerable<ChatUser> GetUsersByRoom(string key)
		{
			Application.Current.Dispatcher.Invoke(() =>
				((MainWindow) Application.Current.MainWindow).WriteToConsole($"Server send list of users in the {key} room."));
			return string.IsNullOrEmpty(key)
				? Enumerable.Empty<ChatUser>()
				: from name in Rooms[key].Users
					select Users[name];
		}

		public ChatRoom GetRoom()
		{
			Application.Current.Dispatcher.Invoke(() =>
				((MainWindow) Application.Current.MainWindow).WriteToConsole($"Server send name of {Clients.Caller.name}'s room."));
			return Clients.Caller.room;
		}

		public ChatRoom GetRoomByKey(string key)
		{
			Application.Current.Dispatcher.Invoke(() =>
				((MainWindow) Application.Current.MainWindow).WriteToConsole($"Server send info about {key} room."));
			return Rooms[key];
		}

		public IEnumerable<ChatRoom> GetRooms()
		{
			Application.Current.Dispatcher.Invoke(() =>
				((MainWindow) Application.Current.MainWindow).WriteToConsole("Server send list of rooms."));
			return Rooms.Values;
		}

		private static string GetMd5Hash(string name) =>
			string.Join("",
				MD5.Create().ComputeHash(
					Encoding.Default.GetBytes(name))
					.Select(b => b.ToString("x2")));

		public ChatUser AddUser(string newUserName, byte color, byte form)
		{
			var user = new ChatUser(newUserName, GetMd5Hash(newUserName))
			{
				ConnectionId = Context.ConnectionId,
				Color = color,
				Form = form
			};
			Users[Context.ConnectionId] = user;
			UserRooms[Context.ConnectionId] = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

			Clients.Caller.name = user.Name;
			Clients.Caller.hash = user.Hash;
			Clients.Caller.id = user.Id;
			Clients.Caller.form = user.Form;
			Clients.Caller.color = user.Color;

			Clients.Caller.addUser(user);

			return user;
		}

		public void SetRoomField(int x, int y)
		{
			Rooms[Clients.Caller.room].FieldX = (byte)x;
			Rooms[Clients.Caller.room].FieldY = (byte)y;
			Rooms[Clients.Caller.room].Matrix = new byte[y,x];
			for (var i = 0; i < y; i++)
			{
				for (var j = 0; j < x; j++)
				{
					Rooms[Clients.Caller.room].Matrix[i, j] = MatrixDefault;
				}
			}
			Application.Current.Dispatcher.Invoke(() =>
				((MainWindow)Application.Current.MainWindow).WriteToConsole($"Field of {Clients.Caller.room} room seted to {x}:{y} "));
		}

		public bool SetDotPosition(int x, int y)
		{
			if ((y < 0) || (x < 0) || (x > Rooms[Clients.Caller.room].FieldX) || (y > Rooms[Clients.Caller.room].FieldY) ||
			    (Rooms[Clients.Caller.room].Matrix[y, x] != 255))
			{
				Application.Current.Dispatcher.Invoke(() =>
					((MainWindow)Application.Current.MainWindow).WriteToConsole($"Error {(x < 0 ? "x<0" : "")} {(y < 0 ? "y<0" : "")} {(x > Rooms[Clients.Caller.room].FieldX ? "x>field" : "")} {(y > Rooms[Clients.Caller.room].FieldY ? "y>field" : "")} {(Rooms[Clients.Caller.room].Matrix[y, x] != 255 ? "dot seted" : "")}."));
				return false;
			}
			Rooms[Clients.Caller.room].Matrix[y, x] = (byte)Clients.Caller.color;
			Application.Current.Dispatcher.Invoke(() =>
				 ((MainWindow)Application.Current.MainWindow).WriteToConsole($"{Clients.Caller.room}: {Clients.Caller.name} set dot to position {x}:{y}"));
			foreach (var user in UserRooms.Where(u => u.Value.Contains(Clients.Caller.room)).Where(user => !user.Key.Equals(Context.ConnectionId)))
			{
				Clients.Client(user.Key).SetDot(x, y, Clients.Caller.color, Clients.Caller.form);
			}
			var queue = (string)Rooms[Clients.Caller.room].Queue.Dequeue();
			Clients.Client(queue).YourTurn();
			Rooms[Clients.Caller.room].Queue.Enqueue(queue);
			return true;
		}

		public void SendField(List<Dot> list)
		{
			Application.Current.Dispatcher.Invoke(() =>
				 ((MainWindow)Application.Current.MainWindow).WriteToConsole(
					 list.Aggregate(
								$"{Clients.Caller.room}: {Clients.Caller.name} close his/her field:\n", 
								(current, dot) => current + $"({dot.X},{dot.Y}) => "
									) + $"\nColor:{Clients.Caller.UserColor}\nForm:{Clients.Caller.UserForm}")
				 );
			foreach (var user in
					UserRooms.Where(u => u.Value.Contains(Clients.Caller.room))
							 .Where(user => !user.Key.Equals(Context.ConnectionId))
					)
				Clients.Client(user.Key).CloseField(list);
		}
	}
}