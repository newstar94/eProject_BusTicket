using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eProject_BusTicket.Models
{
    public class TypeofVehicle
    {
        [Key]
        [Display(Name = "Type")]
        public int TypeID { get; set; }
        [Required]
        [Display(Name = "Type")]
        public string Name { get; set; }
        [Display(Name = "Status")]
        public bool IsActive { get; set; }
        public ICollection<Vehicle> Vehicles { get; set; }
    }
}