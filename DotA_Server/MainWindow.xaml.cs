using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
		private const string ServerUri = "http://localhost:13666";

		public MainWindow()
		{
			InitializeComponent();
		}

		public IDisposable SignalR { get; set; }

		private void Start_OnClick(object sender, RoutedEventArgs e)
		{
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
			catch (TargetInvocationException)
			{
				WriteToConsole("A server is already running at " + ServerUri);
				Dispatcher.Invoke(() => BStart.IsEnabled = true);
				return;
			}
			Dispatcher.Invoke(() => BStop.IsEnabled = true);
			WriteToConsole("Server started at " + ServerUri);
		}

		private void Stop_OnClick(object sender, RoutedEventArgs e)
		{
			BStop.IsEnabled = false;
			BStart.IsEnabled = true;
			SignalR.Dispose();
			WriteToConsole("Server stoped...");
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
		private static readonly ConcurrentDictionary<string, ChatUser> _users =
			new ConcurrentDictionary<string, ChatUser>(StringComparer.OrdinalIgnoreCase);

		private static readonly ConcurrentDictionary<string, HashSet<string>> _userRooms =
			new ConcurrentDictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);

		private static readonly ConcurrentDictionary<string, ChatRoom> _rooms =
			new ConcurrentDictionary<string, ChatRoom>(StringComparer.OrdinalIgnoreCase);

		public void Join(string name)
		{
			AddUser(name);
			Clients.Caller.UserLoggedIn();
			Application.Current.Dispatcher.Invoke(() =>
				((MainWindow) Application.Current.MainWindow).WriteToConsole($"User {name} has id {Context.ConnectionId}"));
		}

		public void SendMessage(string name, string message)
		{
			Clients.All.addMessage(name, message);
			Application.Current.Dispatcher.Invoke(() =>
				((MainWindow) Application.Current.MainWindow).WriteToConsole($"Server send: {name}:{message}"));
		}

		public void SendString(string str)
		{
			Clients.All.addString(str);
			Application.Current.Dispatcher.Invoke(() =>
				((MainWindow) Application.Current.MainWindow).WriteToConsole($"Server send: {str}"));
		}

		public bool SetRoom(string key)
		{
			ChatRoom cr;
			var room = _rooms.TryGetValue(key, out cr);
			if (!room) return false;
			Clients.Caller.room = key;
			Clients.Caller.RoomSeted();
			Application.Current.Dispatcher.Invoke(() =>
				((MainWindow)Application.Current.MainWindow).WriteToConsole($"User {Clients.Caller.name} insert to the {key} room."));
			return true;
		}

		public bool AddRoom(string key)
		{
			var res = _rooms.TryAdd(key, new ChatRoom {Name = key});
			Clients.Caller.RoomAdded();
			if (!res) return false;
			_rooms[key].Users.Add(Context.ConnectionId);
			Application.Current.Dispatcher.Invoke(() =>
				((MainWindow)Application.Current.MainWindow).WriteToConsole($"Room {key} added."));
			return true;
		}

		public bool DeleteRoom(string key)
		{
			ChatRoom room;
			var res = _rooms.TryRemove(key, out room);
			if (!res) return false;
			Application.Current.Dispatcher.Invoke(() =>
				((MainWindow)Application.Current.MainWindow).WriteToConsole($"Room {key} deleted."));
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

			var user = _users.Values.FirstOrDefault(u => u.ConnectionId == Context.ConnectionId);
			if (user == null) return base.OnDisconnected(stopCalled);

			ChatUser ignoredUser;
			_users.TryRemove(user.Name, out ignoredUser);

			// Leave all rooms
			HashSet<string> rooms;
			if (_userRooms.TryGetValue(user.Name, out rooms))
			{
				foreach (var room in rooms)
				{
					Clients.Group(room).leave(user);
					var chatRoom = _rooms[room];
					chatRoom.Users.Remove(user.Name);
				}
			}

			HashSet<string> ignoredRoom;
			_userRooms.TryRemove(user.Name, out ignoredRoom);
			return base.OnDisconnected(stopCalled);
		}

		public IEnumerable<ChatUser> GetUsers() => GetUsersByRoom(Clients.Caller.room);

		public IEnumerable<ChatUser> GetUsersByRoom(string key) => string.IsNullOrEmpty(key)
																		? Enumerable.Empty<ChatUser>()
																		: from name in _rooms[key].Users
																			select _users[name];

		public ChatRoom GetRoom() => Clients.Caller.room;

		public ChatRoom GetRoomByKey(string key) => _rooms[key];

		public IEnumerable<ChatRoom> GetRooms() => _rooms.Values;

		private static string GetMd5Hash(string name) => string.Join("", MD5.Create().ComputeHash(Encoding.Default.GetBytes(name)).Select(b => b.ToString("x2")));

		public ChatUser AddUser(string newUserName)
		{
			var user = new ChatUser(newUserName, GetMd5Hash(newUserName)) {ConnectionId = Context.ConnectionId};
			_users[Context.ConnectionId] = user;
			_userRooms[Context.ConnectionId] = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

			Clients.Caller.name = user.Name;
			Clients.Caller.hash = user.Hash;
			Clients.Caller.id = user.Id;

			Clients.Caller.addUser(user);

			return user;
		}
	}
}