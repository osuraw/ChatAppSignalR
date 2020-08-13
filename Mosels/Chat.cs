using System.Collections.Generic;


namespace Chatapp.Models
{
    public class Chat
    {
        public int Id { get; set; }       
        public ChatType Type { get; internal set; }
        public string Name { get; internal set; }
        public ICollection<Message> Messages { get; set; } 
        public ICollection<ChatUser> ChatUsers { get; set; }

        public Chat()
        {
            Messages = new List<Message>();
            ChatUsers = new List<ChatUser>();
        }
    }
}