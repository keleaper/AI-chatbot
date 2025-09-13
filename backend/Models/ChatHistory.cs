using System;

namespace backend.Models
{
    public class ChatHistory
    {
        public int Id { get; set; }

        // Nullable if you want to allow anonymous entries
        public string? UserId { get; set; }

        // Store messages as serialized JSON (or use a separate table if needed)
        public string? Messages { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
