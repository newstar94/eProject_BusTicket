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
        public int TripID { get; set; }
        public virtual Trip Trip { get; set; }
        public bool IsActive { get; set; }
        [Display(Name = "Thời gian khởi hành")]
        public DateTime DepartureTime { get; set; }
    }
}