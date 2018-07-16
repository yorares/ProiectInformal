using Newtonsoft.Json;
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
        [JsonIgnore]
        public ICollection<Review> Review { get; set; }
    }
    public enum BadgeType
    {
        Bad = -1,
        Neutral = 0,
        Good =1
    }
}
