using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using eProject_BusTicket.Enum;

namespace eProject_BusTicket.Models
{
    public class BookingTicket
    {
        public int BookingTicketID { get; set; }
        public int TripID { get; set; }
        public virtual Trip Trip { get; set; }
        public int BookingID { get; set; }
        public virtual Booking Booking { get; set; }
        public decimal Price { get; set; }
        public DateTime Date { get; set; }
        public string PassengerName { get; set; }
        public int PassengerAge { get; set; }
        public TicketStatus Status { get; set; }
    }
}