using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Stark.Models
{
    public partial class Badge
    {
        public Badge()
        {
            Review = new HashSet<Review>();
        }

        public int BadgeId { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public int Type { get; set; }
        [Required]
        public string Description { get; set; }
        [JsonIgnore]
        public ICollection<Review> Review { get; set; }
    }
    public enum BadgeType
    {
        Awful = 0,
        Bad = 1,
        Good =3,
        Excellent= 4


    }
}
