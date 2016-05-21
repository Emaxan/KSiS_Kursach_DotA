using System;
using System.Reflection;
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
			app.UseCors(CorsOptions.AllowAll);
			app.MapSignalR();
		}
	}

	public class MyHub : Hub
	{
		public void SendMessage(string name, string message)
		{
			Clients.All.addMessage(name, message, Users.FindById(Context.ConnectionId).Room);
			Application.Current.Dispatcher.Invoke(() =>
				((MainWindow) Application.Current.MainWindow).WriteToConsole($"Client send: {name}:{message}"));
		}

		public void SendString(string str)
		{
			Clients.All.addString(str, Users.FindById(Context.ConnectionId).Room);
			Application.Current.Dispatcher.Invoke(() =>
				((MainWindow) Application.Current.MainWindow).WriteToConsole($"Client send: {str}"));
		}

		//public void SetRoom(int room)	
		//{
		//	Users.FindById(Context.ConnectionId).Room = room;
		//}

		public override Task OnConnected()
		{
			Application.Current.Dispatcher.Invoke(() =>
				((MainWindow) Application.Current.MainWindow).WriteToConsole("Client connected: " + Context.ConnectionId));
			//Users.AddUser(new User(Context.ConnectionId));
			return base.OnConnected();
		}

		public override Task OnDisconnected(bool stopCalled)
		{
			Application.Current.Dispatcher.Invoke(() =>
				((MainWindow) Application.Current.MainWindow).WriteToConsole("Client disconnected: " + Context.ConnectionId));
			//Users.RemoveUserWithId(Context.ConnectionId);
			return base.OnDisconnected(stopCalled);
		}

		public void Show(string mes)
		{
			Application.Current.Dispatcher.Invoke(() =>
				((MainWindow) Application.Current.MainWindow).WriteToConsole(mes));
		}

		public void SendClients()
		{
			SendString("Cients :-)");
			Application.Current.Dispatcher.Invoke(() =>
				((MainWindow) Application.Current.MainWindow).WriteToConsole("Client requests list of clients."));
		}
	}

	public static class Users
	{
		private static int _size;
		private static User[] _users = new User[0];

		public static void AddUser(User user)
		{
			Array.Resize(ref _users, ++_size);
			_users[_size - 1] = user;
		}

		public static void RemoveUserWithId(string id)
		{
			var i = 0;
			while (!Equals(_users[i].ConnectionId, id)) i++;
			for (; i < _size - 1; i++) _users[i] = _users[i + 1];
		}

		public static User FindById(string id)
		{
			var i = 0;
			while (!Equals(_users[i].ConnectionId, id)) i++;
			return _users[i];
		}

	}

	public class User
	{
		public User(string connextionId)
		{
			ConnectionId = connextionId;
		}

		public string ConnectionId { get; set; }
		public int Room { get; set; } = -1;
	}
}