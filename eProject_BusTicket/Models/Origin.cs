using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eProject_BusTicket.Models
{
    public class Origin
    {
        public int OriginID { get; set; }
        [Required]
        [Display(Name = "Origin" )]
        public string Name { get; set; }
        public ICollection<Trip> Trips { get; set; }
    }
}