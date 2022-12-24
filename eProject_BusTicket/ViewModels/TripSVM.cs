using eProject_BusTicket.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eProject_BusTicket.ViewModels
{
    public class TripSVM
    {
        public TripSchedule TripSchedule { get; set; }
        public List<Station> Stations { get; set; }
        public List<RouteSchedule> RouteSchedules { get; set; }
    }
}