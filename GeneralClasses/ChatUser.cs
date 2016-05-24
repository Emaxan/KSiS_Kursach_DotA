using System;

namespace GeneralClasses
{
	[Serializable]
	public class ChatUser
	{
		public string ConnectionId { get; set; }
		public string Id { get; set; }
		public string Name { get; set; }
		public string Hash { get; set; }
		public byte Color { get; set; }
		public byte Form { get; set; }

		public ChatUser()
		{
		}

		public ChatUser(string name, string hash)
		{
			Name = name;
			Hash = hash;
			Id = Guid.NewGuid().ToString("d");
		}
	}
}
