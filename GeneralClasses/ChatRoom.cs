using System;
using System.Collections;
using System.Collections.Generic;

namespace GeneralClasses
{
	public class ChatRoom
	{
		public ChatRoom()
		{
			Messages = new List<ChatMessage>();
			Users = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		}

		public Queue<string> Queue;
		public byte[,] Matrix;
		public string Name { get; set; }
		public List<ChatMessage> Messages { get; set; }
		public HashSet<string> Users { get; set; }
		public byte FieldX { get; set; }
		public byte FieldY { get; set; }

		public override string ToString() => Name;
	}
}