using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using eProject_BusTicket.Data;

namespace eProject_BusTicket.Models
{
    public class Trip
    {
        public int TripID { get; set; }
        public string Name { get; set; }
        public int VehicleID { get; set; }
        public virtual Vehicle Vehicle { get; set; }
        public int OriginID { set; get; }
        public virtual Origin Origin { get; set; }
        public int DestinationID { set; get; }
        public virtual Destination Destination { get; set; }
        public DateTime DepartureTime { get; set; }
        public string TripTime { get; set; }
        public int Distance { get; set; }
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal Price { get; set; }
        public int AvailableSeat { get; set; }
    }
}