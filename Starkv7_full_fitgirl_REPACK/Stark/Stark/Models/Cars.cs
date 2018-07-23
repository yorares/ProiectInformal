using System;
using System.Collections.Generic;

namespace Stark.Models
{
    public partial class Cars
    {
        public Cars()
        {
            Review = new HashSet<Review>();
        }

        public int LicenceId { get; set; }
        public string Plate { get; set; }

        public ICollection<Review> Review { get; set; }
    }
}
