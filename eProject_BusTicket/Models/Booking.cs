using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eProject_BusTicket.Models
{
    public class Booking
    {
        public int BookingID { get; set; }
        public int BookingVNPID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public decimal TotalPayment { get; set; }
        public string BookingCode { get; set; }
        public DateTime DateTime { get; set; }
        public string Status { get; set; }
        public long TranId { get; set; }
        public int AccountID { get; set; }
        public virtual Account Account { get; set; }
        public ICollection<BookingTicket> BookingTickets { get; set; }
    }
}