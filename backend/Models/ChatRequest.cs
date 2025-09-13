// Chat Request is the user sending a chat

namespace backend.Models
{
    // Message sent
    public class ChatRequest
    {
        // List of messages in the chat history
        // Each message has a role (user or assistant) and content (the actual message text)
        // This allows the AI to understand the context of the conversation
        public int Id { get; set; } // Unique identifier for the chat request
        public string? Message { get; set; }
        public List<RequestMessage> Messages { get; set; } = new List<RequestMessage>(); // In ChatBox.jsx aswell as
        public int? SessionId { get; set; } // Optional session ID to associate with a chat session
    }

    public class RequestMessage
    {
        public int Id { get; set; } // Unique identifier for the message
        public string? role { get; set; } // "user" or "assistant"
        public string? content { get; set; } // the actual message content
    }
}