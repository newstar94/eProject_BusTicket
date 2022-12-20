using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace eProject_BusTicket.ViewModes
{
    public class Location
    {
        public int LocationID { get; set; }
        [Required]
        [Display(Name = "Location")]
        public string Name { get; set; }
    }
}