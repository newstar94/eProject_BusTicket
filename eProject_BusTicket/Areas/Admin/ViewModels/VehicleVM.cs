using eProject_BusTicket.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eProject_BusTicket.Areas.Admin.ViewModels
{
    public class VehicleVM
    {
        [Display(Name = "Seats")]
        public int Seats { get; set; }
        [Display(Name = "Price/km")]
        [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = true)]
        public decimal Price { get; set; }
        [Display(Name = "Vehicle Code")]
        public string Code { get; set; }
        public string Type { get; set; }
    }
}