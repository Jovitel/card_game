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

        public int Wins { get; set; } = 0;
        public int Losses { get; set; } = 0;
        public int ExperiencePoints { get; set; } = 0;  // XP
        public int Level { get; set; } = 1;              // Lygis
        public List<int> Friends { get; set; } = new(); // saugomi draugų ID

        public string AvatarUrl { get; set; } = "images/default_avatar.png";
        public string SelectedCardColor { get; set; } = "red"; // Kortų nugarėlė
        public string SelectedTableColor { get; set; } = "#2c425a"; // Stalo fonas
        public string ThemePreference { get; set; } = "dark"; // tema
    }
}
