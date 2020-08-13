

using System;

namespace Chatapp.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string  Text { get; set; }
        public DateTime TimeStamp { get; set; }
        public int ChatId { get; internal set; }
        public Chat Chat {get;set;} 
    }
}