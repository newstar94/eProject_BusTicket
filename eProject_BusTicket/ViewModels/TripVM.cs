using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eProject_BusTicket.Data;
using eProject_BusTicket.Models;

namespace eProject_BusTicket.ViewModels
{
    public class TripVM
    {
        public TypeofVehicle TypeofVehicleDetails { get; set; }
        public Vehicle VehicleDetails { get; set; }
        public Trip TripDetails { get; set; }
    }
}