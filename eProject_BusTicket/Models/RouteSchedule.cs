using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eProject_BusTicket.Models
{
    public class RouteSchedule
    {
        public int RouteScheduleID { get; set; }
        public int TripScheduleID { get; set; }
        public string RouteScheduleCode { get; set; }
        public int RouteID { get; set; }
        public virtual Route Route { get; set; }
        [Display(Name = "Avaiable Seats")]
        public int AvaiableSeat { get; set; }
        [DisplayFormat(DataFormatString = "{0:HH:mm dd/MM/yyyy}")]
        [Display(Name = "Departure Time")]
        public DateTime DepartureTime { get; set; }
    }
}