using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eProject_BusTicket.Models
{
    public class Location
    {
        public int LocationID { get; set; }
        [Required]
        [Display(Name = "Location")]
        public string LocationName { get; set; }
        [Display(Name = "Status")]
        public bool IsActive { get; set; }
    }
}