using eProject_BusTicket.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eProject_BusTicket.ViewModels
{
    public class BookingVM
    {
        public RouteSchedule RouteDetails { get; set; }
        public TripSchedule TripDetails { get; set; }
        public string PassengerName { get; set; }
        public int PassengerAge { get; set; }
        public decimal Price { get; set; }
    }
}