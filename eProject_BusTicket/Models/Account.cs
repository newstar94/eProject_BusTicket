using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eProject_BusTicket.Models
{
    public class Account
    {
        public int AccountID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool Role { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; }
    }
}