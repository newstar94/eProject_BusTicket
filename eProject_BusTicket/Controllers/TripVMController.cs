using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eProject_BusTicket;
using eProject_BusTicket.Data;
using eProject_BusTicket.Models;
using eProject_BusTicket.ViewModels;

namespace eProject_BusTicket.Controllers
{
    public class TripVMController : Controller
    {
        private AppDbContext db = new AppDbContext();
        // GET: TripVM
        public ActionResult Index()
        {
            List<TypeofVehicle> TypeofVehicles = db.TypeofVehicles.ToList();
            List<Vehicle> Vehicles = db.Vehicles.ToList();
            List<Trip> Trips = db.Trips.ToList();
            var multitable = from t in TypeofVehicles
                             join v in Vehicles on t.TypeID equals v.TypeID into table1
                             from v in table1
                             join tr in Trips on v.VehicleID equals tr.VehicleID into table2
                             from tr in table2
                             select new TripVM
                             {
                                 TypeofVehicleDetails = t,
                                 VehicleDetails = v,
                                 TripDetails = tr
                             };
            return View(multitable);
        }
    }
}