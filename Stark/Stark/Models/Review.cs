using System;
using System.Collections.Generic;

namespace Stark.Models
{
    public partial class Review
    {
        private string userip;
        public int ReviewId { get; set; }
        public int LicenceId { get; set; }
        public int BadgeId { get; set; }
        public DateTime CreateDate { get; set; }
        public string UserIp
        {
            get
            {
                return userip;
            }
            set
            {
                userip = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList.GetValue(0).ToString();
            }
        }


        public Badge Badge { get; set; }
        public Cars Licence { get; set; }


    }


}
