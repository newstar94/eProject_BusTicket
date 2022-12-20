using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eProject_BusTicket.Models
{
    public class Destination
    {
        public int DestinationID { get; set; }
        [Required]
        [Display(Name = "Destination")]
        public string Name { get; set; }
        public ICollection<Trip> Trips { get; set; }
    }
}