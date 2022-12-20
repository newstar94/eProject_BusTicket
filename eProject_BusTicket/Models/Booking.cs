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
        public bool TypeofPayment { get; set; }
        public DateTime DateTime { get; set; }
        public bool Status { get; set; }
        public ICollection<BookingTicket> BookingTickets { get; set; }
    }
}