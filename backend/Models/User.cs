namespace backend.Models
{
    public class User
    {
        public int Id { get; set; } // Unique identifier for the user
        public string Username { get; set; } = null!; // Name of the user (Making them non-nullable)
        public byte[] PasswordHash { get; set; } = null!; // Password hashing of the user
        public byte[] PasswordSalt { get; set; } = null!; // Salt for password hashing
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public List<ChatMessage> Messages { get; set; } = new(); // Timestamp when the user was createdq
    }
}