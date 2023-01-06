using eProject_BusTicket.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eProject_BusTicket.ViewModels
{
    public class TripVM
    {
        public Trip Trip { get; set; }
        public List<Station> Stations { get; set; }
        public List<Route> Routes { get; set; }
        public string VehicleCode { get; set; }
        public string TripCode { get; set; }
        public string Type { get; set; }
        public int Seats { get; set; }
        public decimal Price { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public List<string> StationList { get; set;}
    }
}