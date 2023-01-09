﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eProject_BusTicket.Models
{
    public class TimeUsed
    {
        public int TimeUsedID { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public int VehicleID { get; set; }
        public int TripScheduleID { get; set; }
        public Vehicle Vehicle { get; set; }
    }
}