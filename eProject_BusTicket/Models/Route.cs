using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eProject_BusTicket.Models
{
    public class Route
    {
        public int RouteID { get; set; }
        public int StartID { get; set; }
        public int EndID { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
        public float Distance { get; set; }
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal Price { get; set; }
        public int AvaiableSeats { get; set; }
        public int Duration { get; set; }
        public int TripID { get; set; }
        public virtual Trip Trip { get; set; }
        public virtual ICollection<RouteSchedule> RouteSchedules { get; set; }
    }
}