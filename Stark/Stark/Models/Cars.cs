using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Stark.Models
{
    public partial class Cars
    {
        public Cars()
        {
            Review = new HashSet<Review>();
        }

        public int LicenceId { get; set; }
        [Required]
        [StringLength(7, MinimumLength =6)]
        public string Plate { get; set; }

        public ICollection<Review> Review { get; set; }
    }
}
