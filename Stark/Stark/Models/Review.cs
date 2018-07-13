using System;
using System.Collections.Generic;

namespace Stark.Models
{
    public partial class Review
    {
        public int ReviewId { get; set; }
        public int LicenceId { get; set; }
        public int BadgeId { get; set; }
        public DateTime CreateDate { get; set; }
        public string UserIp { get; set; }

        public Badge Badge { get; set; }
        public Cars Licence { get; set; }
    }
}
