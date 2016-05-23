using System;
using System.Collections.Generic;

namespace DotA_Server
{
	public class ChatRoom
	{
		public string Name { get; set; }
		public List<ChatMessage> Messages { get; set; }
		public HashSet<string> Users { get; set; }

		public ChatRoom()
		{
			Messages = new List<ChatMessage>();
			Users = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		}

		public override string ToString() => Name;
	}
}