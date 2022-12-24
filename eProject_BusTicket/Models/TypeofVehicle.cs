using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eProject_BusTicket.Data
{
    public class TypeofVehicle
    {
        [Key]
        public int TypeID { get; set; }
        [Required]
        [Display(Name = "Loại xe")]
        public string Name { get; set; }
        public ICollection<Vehicle> Vehicles { get; set; }
    }
}