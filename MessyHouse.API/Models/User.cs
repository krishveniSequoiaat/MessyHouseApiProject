namespace MessyHouseAPIProject.Models
{
    public class User
    {
        public int UserId { get; set; }      // PK
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
