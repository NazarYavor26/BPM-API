using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPM.DAL.Entities
{
    public class UserProfile
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(150)]
        public string LastName { get; set; }

        [Required]
        public int UserId { get; set; }

        public User User { get; set; }

        [Required]
        public Enums.Level LevelId { get; set; }

        public virtual Level Level { get; set; }

        public int? LastUpdatedById { get; set; }

        public virtual User LastUpdatedBy { get; set; }

        public int? CompetenceLeadId { get; set; }

        public virtual User CompetenceLead { get; set; }
    }
}
