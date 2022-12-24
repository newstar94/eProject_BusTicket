using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eProject_BusTicket.Models
{
    public class Station
    {
        public int StationID { get; set; }
        public string StationAdress { get; set; }
        public int TripID { get; set; }
        public virtual Trip Trip { get; set; }
    }
}