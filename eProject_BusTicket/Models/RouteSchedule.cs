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
        public int RouteID { get; set; }
        public virtual Route Route { get; set; }
        [Display(Name = "Số ghế trống")]
        public int AvaiableSeat { get; set; }
        [Display(Name = "Thời gian khởi hành")]
        public DateTime DepartureTime { get; set; }
    }
}