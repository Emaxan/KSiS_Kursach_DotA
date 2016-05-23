using System;

namespace DotA_Server
{
	[Serializable]
	public class ChatMessage
	{
		public string Id { get; private set; }
		public string User { get; set; }
		public string Text { get; set; }
		public ChatMessage(string user, string text)
		{
			User = user;
			Text = text;
			Id = Guid.NewGuid().ToString("d");
		}
	}
}