using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using eProject_BusTicket.Models;

namespace eProject_BusTicket.ViewModels
{
    public class BookingVM
    {
        public Trip TripDetails { get; set; }
        public string PassengerName { get; set; }
        public int PassengerAge { get; set; }
        public decimal Price { get; set; }
    }
}