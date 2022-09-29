using System.ComponentModel.DataAnnotations;

namespace BPM.DAL.Entities
{
    public class RefreshToken
    {
        public int Id { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        public DateTime ExpiredIn { get; set; }

        public bool IsExpired => DateTime.UtcNow >= ExpiredIn;

        public int UserId { get; set; }

        public virtual User User { get; set; }
    }
}
