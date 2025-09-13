// backend/Models/ChatMessage.cs
namespace backend.Models
{
    public class ChatMessage
    {
        public int Id { get; set; } // Primary key
        public int ChatSessionId { get; set; }      // link to ChatSession
        public string Role { get; set; } = null!;
        public string Content { get; set; } = null!;
        public DateTime Timestamp { get; set; }

        public ChatSession? ChatSession { get; set; }
    }
}
