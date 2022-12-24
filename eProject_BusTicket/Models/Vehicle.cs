﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using eProject_BusTicket.Models;

namespace eProject_BusTicket.Data
{
    public class Vehicle
    {
        public int VehicleID { get; set; }
        [Required]
        [Display(Name = "Số ghế")]
        [Range(4,100)]
        public int Seats { get; set; }
        [Required]
        [Display(Name = "Giá/km")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
        [Display(Name = "Mã xe")]
        public string Code { get; set; }
        public int TypeID { get; set; }
        public virtual TypeofVehicle TypeofVehicle { get; set; }
        public ICollection<Trip> Trips { get; set; }
    }
}