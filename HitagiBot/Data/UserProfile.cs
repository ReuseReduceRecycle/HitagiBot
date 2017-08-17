using System.ComponentModel.DataAnnotations;

namespace HitagiBot.Data
{
    public class UserProfile
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [MaxLength(16)]
        public string LastFm { get; set; }
    }
}