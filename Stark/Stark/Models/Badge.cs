using System;
using System.Collections.Generic;

namespace Stark.Models
{
    public partial class Badge
    {
        public Badge()
        {
            Review = new HashSet<Review>();
        }

        public int BadgeId { get; set; }
        public string Title { get; set; }
        public int Type { get; set; }
        public string Description { get; set; }

        public ICollection<Review> Review { get; set; }
    }
}
