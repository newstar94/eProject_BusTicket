using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eProject_BusTicket.Models
{
    public class Vehicle
    {
        [Display(Name = "Vehicle")]
        public int VehicleID { get; set; }
        [Required]
        [Display(Name = "Seats")]
        [Range(4, 100)]
        public int Seats { get; set; }
        [Required]
        [Display(Name = "Price/km")]
        [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = true)]
        public decimal Price { get; set; }
        [Display(Name = "Status")]
        public bool IsActive { get; set; }
        [Display(Name = "Vehicle Code")]
        public string Code { get; set; }
        public int TypeID { get; set; }
        public virtual TypeofVehicle TypeofVehicle { get; set; }
        public ICollection<Trip> Trips { get; set; }
    }
}