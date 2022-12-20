using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using eProject_BusTicket.Models;

namespace eProject_BusTicket.Data
{
    public class Vehicle
    {
        public int VehicleID { get; set; }
        public int Seats { get; set; }
        [DisplayFormat(DataFormatString = "{0:C0}")]
        public decimal Price { get; set; }
        public string Code { get; set; }
        public int TypeID { get; set; }
        public virtual TypeofVehicle TypeofVehicle { get; set; }
        public ICollection<Trip> Trips { get; set; }
    }
}