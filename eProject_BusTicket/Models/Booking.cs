using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eProject_BusTicket.Models
{
    public class Booking
    {
        public int BookingID { get; set; }
        public decimal TotalPayment { get; set; }
        public DateTime DateTime { get; set; }
        public ICollection<BookingTicket> BookingTickets { get; set; }
    }
}