using System.ComponentModel.DataAnnotations;

namespace HitagiBot.Data
{
    public class UserLocation
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public double Latitude { get; set; }

        [Required]
        public double Longitude { get; set; }

        [Required]
        public string FormattedAddress { get; set; }

        [Required]
        public bool IsAmerica { get; set; }
    }
}