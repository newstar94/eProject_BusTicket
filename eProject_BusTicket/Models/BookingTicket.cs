using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using eProject_BusTicket.Areas.Admin.Enum;

namespace eProject_BusTicket.Models
{
    public class BookingTicket
    {
        public int BookingTicketID { get; set; }
        public int RouteScheduleID { get; set; }
        public virtual RouteSchedule RouteSchedule { get; set; }
        public int BookingID { get; set; }
        public virtual Booking Booking { get; set; }
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal Price { get; set; }
        [DisplayFormat(DataFormatString = "{0:HH:mm dd/MM/yyyy}")]
        public DateTime DepartureTime { get; set; }
        public string PassengerName { get; set; }
        public int PassengerAge { get; set; }
        public TicketStatus Status { get; set; }
    }
}