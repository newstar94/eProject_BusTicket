using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace eProject_BusTicket.Models
{
    public class Booking
    {
        public int BookingID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal TotalPayment { get; set; }
        public string BookingCode { get; set; }
        [DisplayFormat(DataFormatString = "{0:HH:mm:ss dd/MM/yyyy}")]
        public DateTime DateTime { get; set; }
        public string Status { get; set; }
        public long TranId { get; set; }
        public string UserID { get; set; }
        public ICollection<BookingTicket> BookingTickets { get; set; }
    }
}