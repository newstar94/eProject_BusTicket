using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eProject_BusTicket.Models
{
    public class Trip
    {
        public int TripID { get; set; }
        [Required]
        [Display(Name = "Mã chuyến")]
        public string CodeName { get; set; }
        public int VehicleID { get; set; }
        public virtual Vehicle Vehicle { get; set; }
        [Required]
        [Display(Name = "Đầu tuyến")]
        public string Origin { set; get; }
        [Required]
        [Display(Name = "Cuối tuyến")]
        public string Destination { set; get; }
        public bool IsActive { set; get; }
        public virtual ICollection<Station> Stations { get; set; }
        public virtual ICollection<Route> Routes { get; set; }
        public virtual ICollection<TripSchedule> TripSchedules { get; set; }
    }
}