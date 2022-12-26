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
        [Display(Name = "Tên địa điểm")]
        public string LocationName { get; set; }
        //public string Name { get; set; }
    }
}