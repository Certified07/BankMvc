namespace BankMvc.Models.Entity
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Salt { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime LastLoginDate { get; internal set; }

        // Removed invalid implicit operator with 'void' parameter type  
        // If you need an implicit operator, define it with a valid type.  
        // Example:  
        public static implicit operator User(string username)
        {
            return new User { Username = username };
        }
    }
}
