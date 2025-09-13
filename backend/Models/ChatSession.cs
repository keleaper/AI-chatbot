using System;
using System.Collections.Generic;

namespace backend.Models
{
    public class ChatSession
    {
        public int Id { get; set; }
        public int UserId { get; set; }             // owner of the session
        public User? User { get; set; }
        public string? Title { get; set; } // title of the chat session
        public DateTime LastUpdated { get; set; } // Last time a message was added
        public List<ChatMessage> Messages { get; set; } = new();
    }
}