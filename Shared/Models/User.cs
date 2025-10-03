using System.ComponentModel.DataAnnotations;

namespace CardGame.Shared.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }   // Primary key

        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;
    }
}
