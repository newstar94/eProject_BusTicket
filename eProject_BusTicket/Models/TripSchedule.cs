using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eProject_BusTicket.Models
{
    public class TripSchedule
    {
        public int TripScheduleID { get; set; }
        public string TripScheduleCode { get; set; }
        public int TripID { get; set; }
        public virtual Trip Trip { get; set; }
        [DisplayFormat(DataFormatString = "{0:HH:mm dd/MM/yyyy}")]
        [Display(Name = "Departure Time")]
        public DateTime DepartureTime { get; set; }
        public DateTime Date { get; set; }
    }
}