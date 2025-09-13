// Chat Response is what the AI responds with

using System.Collections.Generic;

namespace backend.Models
{
    // Response returned
    public class ChatResponse
    {
        public string? Id { get; set; } // Unique identifier for the chat response
        public string? ResponseText { get; set; } // The actual response text from the AI
        public List<Choice> choices { get; set; } = new();
        // create a new list item in the choices list
    }

    public class Choice
    {
        public string? Id { get; set; } // Unique identifier for the choice
        public ResponseMessage message { get; set; } = new();
        // creates a new message that have a role/"assistant", and content/"what is said"
    }

    public class ResponseMessage
    {
        public string? Id { get; set; } // Unique identifier for the response message
        public string? role { get; set; } // usually "assistant"
        public string? content { get; set; } // AI's response text
    }
}