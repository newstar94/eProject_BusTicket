using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using eProject_BusTicket.Models;
using eProject_BusTicket.ViewModels;

namespace eProject_BusTicket.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TripSchedulesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// GET: TripSchedules
        public ActionResult Index(int? Origin, int? Destination, DateTime? dateTime)
        {
            var origin = "";
            var destination = "";
            if (Origin != null)
            {
                origin = db.Locations.Find(Origin).LocationName;

            }
            if (Destination != null)
            {
                destination = db.Locations.Find(Destination).LocationName;
            }

            ViewBag.Location = new SelectList(db.Locations, "LocationID", "LocationName");
            List<TripSVM> tripSVMs = new List<TripSVM>();
            var tripSchedules = db.TripSchedules
                .Where(t => t.Trip.Origin == origin || Origin == null)
                .Where(t => t.Trip.Destination == destination || Destination == null)
                .Where(t => t.Date == dateTime || dateTime == null)
                .Include(t => t.Trip).ToList();
            var routeSchedules = db.RouteSchedules.ToList();
            var stations = db.Stations.ToList();

            foreach (var tripSchedule in tripSchedules)
            {
                TripSVM tripSVM = new TripSVM();
                tripSVM.TripSchedule = tripSchedule;
                tripSVM.RouteSchedules = routeSchedules.Where(rs => rs.TripScheduleID == tripSchedule.TripScheduleID).ToList();
                tripSVM.Stations = stations.Where(st => st.TripID == tripSchedule.TripID).ToList();
                tripSVMs.Add(tripSVM);
            }

            return View(tripSVMs.ToList());
        }

        // GET: TripSchedules/Details/5
        public ActionResult Details(int? id)
        {
            var stations = db.Stations.ToList();
            var routeSchedules = db.RouteSchedules.ToList();
            TripSVM tripSVM = new TripSVM();
            TripSchedule tripSchedule = db.TripSchedules.Find(id);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (tripSchedule == null)
            {
                return HttpNotFound();
            }
            tripSVM.TripSchedule = tripSchedule;
            tripSVM.RouteSchedules = routeSchedules.Where(rs => rs.TripScheduleID == tripSchedule.TripScheduleID).ToList();
            tripSVM.Stations = stations.Where(st => st.TripID == tripSchedule.TripID).ToList();
            return View(tripSVM);
        }

        // GET: TripSchedules/Create
        public ActionResult Create()
        {
            ViewBag.TripID = new SelectList(db.Trips.Where(t => t.IsActive == true), "TripID", "CodeName");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TripScheduleID,TripID,DepartureTime")] TripSchedule tripSchedule)
        {
            if (ModelState.IsValid)
            {
                var trip = db.Trips.Find(tripSchedule.TripID);
                tripSchedule.TripScheduleCode = trip.CodeName + tripSchedule.DepartureTime.ToString("ddMMyyHHmm");
                tripSchedule.Date = tripSchedule.DepartureTime.Date;
                db.TripSchedules.Add(tripSchedule);
                db.SaveChanges();
                var routes = db.Routes.Where(r => r.TripID == tripSchedule.TripID).ToList();
                List<Route> newroutes = new List<Route>();
                for (int i = 0; i < routes.Count - 1; i++)
                {
                    if (routes[i].StartID == routes[i + 1].StartID)
                    {
                        Route route = routes[i];
                        newroutes.Add(route);
                    }
                }
                newroutes = routes.Except(newroutes).ToList();
                List<Station> stations = new List<Station>();

                for (int i = 0; i < newroutes.Count; i++)
                {
                    Station station = new Station();
                    station.StationID = newroutes[i].StartID;
                    station.StationAdress = newroutes[i].Start;
                    stations.Add(station);
                }

                for (int i = 0; i < routes.Count; i++)
                {
                    RouteSchedule routeSchedule = new RouteSchedule();
                    routeSchedule.TripScheduleID = tripSchedule.TripScheduleID;
                    routeSchedule.RouteID = routes[i].RouteID;
                    routeSchedule.Route = routes[i];
                    routeSchedule.RouteScheduleCode = tripSchedule.TripScheduleCode + i;
                    routeSchedule.AvaiableSeat = routes[i].AvaiableSeats;
                    if (routes[i].StartID == stations[0].StationID)
                    {
                        routeSchedule.DepartureTime = tripSchedule.DepartureTime.AddMinutes(0);
                    }
                    else
                    {
                        for (int j = 1; j < stations.Count; j++)
                        {
                            if (routes[i].StartID == stations[j].StationID)
                            {
                                var duration = routes.Where(r => r.StartID == stations[0].StationID)
                                    .Where(r => r.EndID == stations[j].StationID).ToList().FirstOrDefault().Duration;

                                routeSchedule.DepartureTime = tripSchedule.DepartureTime.AddMinutes(duration + j * 7);
                            }
                        }
                    }
                    db.RouteSchedules.Add(routeSchedule);
                    db.SaveChanges();
                }


                return RedirectToAction("Index");
            }

            ViewBag.TripID = new SelectList(db.Trips.Where(t => t.IsActive == true), "TripID", "CodeName", tripSchedule.TripID);
            return View(tripSchedule);
        }

        public JsonResult Gettrip(int TripID)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var trip = db.Trips.Find(TripID);
            var stations = db.Stations.Where(s => s.TripID == trip.TripID).ToList();
            List<string> StationList = new List<string>();
            foreach (var station in stations)
            {
                StationList.Add(station.StationAdress);
            }
            TripVM tripVm = new TripVM();
            tripVm.Destination = trip.Destination;
            tripVm.Origin = trip.Origin;
            tripVm.TripCode = trip.CodeName;
            var vehicle = db.Vehicles.Find(trip.VehicleID);
            tripVm.Price = vehicle.Price;
            tripVm.Seats = vehicle.Seats;
            tripVm.VehicleCode = vehicle.Code;
            var type = db.TypeofVehicles.Find(vehicle.TypeID);
            tripVm.Type = type.Name;
            tripVm.StationList = StationList;
            return Json(tripVm, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            TripSchedule trip = db.TripSchedules.Find(id);
            db.TripSchedules.Remove(trip);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}