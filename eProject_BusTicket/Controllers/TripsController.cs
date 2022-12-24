using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using eProject_BusTicket.Data;
using eProject_BusTicket.Models;
using eProject_BusTicket.ViewModels;

namespace eProject_BusTicket.Controllers
{
    public class TripsController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: Trips
        public ActionResult Index()
        {
            List<TripVM> tripVMs = new List<TripVM>();
            var trips = db.Trips.Include(t => t.Vehicle).ToList();
            var routes = db.Routes.ToList();
            var stations = db.Stations.ToList();
            foreach (var trip in trips)
            {
                TripVM tripvm = new TripVM();
                tripvm.Trip=trip;
                tripvm.Routes = routes.Where(r => r.TripID == trip.TripID).ToList();
                tripvm.Stations = stations.Where(st => st.TripID == trip.TripID).ToList();
                tripVMs.Add(tripvm);
            }
            return View(tripVMs.ToList());
        }

        // GET: Trips/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Trip trip = db.Trips.Find(id);
            if (trip == null)
            {
                return HttpNotFound();
            }
            TripVM tripvm = new TripVM();
            tripvm.Trip = trip;
            var stations = db.Stations.Where(st => st.TripID == trip.TripID);
            ViewBag.Station = new SelectList(stations, "StationId", "StationAdress");
            return View(tripvm);
        }

        public JsonResult Getvehicle(int TypeID)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var vehicles = db.Vehicles.Where(v => v.TypeID == TypeID).ToList();
            return Json(vehicles, JsonRequestBehavior.AllowGet);
        }

        // GET: Trips/Create
        public ActionResult Create()
        {
            ViewBag.VehicleID = new SelectList(db.Vehicles.Where(v=>v.IsActive==true), "VehicleID", "Code");
            ViewBag.Location = new SelectList(db.Locations, "LocationID", "LocationName");
            ViewBag.TypeID = new SelectList(db.TypeofVehicles, "TypeID", "Name");
            return View();
        }

        // POST: Trips/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create(TripVM tripvm)
        {
            if (ModelState.IsValid)
            {
                Trip trip = new Trip();
                trip.VehicleID=tripvm.Trip.VehicleID;
                trip.CodeName = tripvm.Trip.CodeName;
                trip.Origin = tripvm.Trip.Origin;
                trip.Destination = tripvm.Trip.Destination;
                trip.IsActive = true;
                db.Trips.Add(trip);
                db.SaveChanges();
                for (int i = 0; i < tripvm.Stations.Count; i++)
                {
                    Station station = new Station();
                    station.TripID = trip.TripID;
                    station.StationAdress = tripvm.Stations[i].StationAdress;
                    db.Stations.Add(station);
                    db.SaveChanges();
                }

                Vehicle vehicle = db.Vehicles.Find(trip.VehicleID);
                var liststaition = db.Stations.Where(s => s.TripID == trip.TripID).ToList();
                for (int i = 0; i < liststaition.Count; i++)
                {
                    for (int j = liststaition.Count - 1; j > i; j--)
                    {
                        Route route = new Route();
                        route.TripID = trip.TripID;
                        route.Start = liststaition[i].StationAdress;
                        route.StartID = liststaition[i].StationID;
                        route.End = liststaition[j].StationAdress;
                        route.EndID = liststaition[j].StationID;
                        for (int k = i; k < j; k++)
                        {
                            route.Distance += tripvm.Routes[k].Distance;
                            route.Duration += tripvm.Routes[k].Duration;
                        }

                        route.Price = (decimal)route.Distance * vehicle.Price;
                        route.AvaiableSeats = vehicle.Seats;
                        db.Routes.Add(route);
                        db.SaveChanges();
                    }
                }

                return Json(Url.Action("Index"));
            }
            ViewBag.VehicleID = new SelectList(db.Vehicles.Where(v => v.IsActive == true), "VehicleID", "Code", tripvm.Trip.VehicleID);
            return View(tripvm);
        }

        // GET: Trips/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Trip trip = db.Trips.Find(id);
            if (trip == null)
            {
                return HttpNotFound();
            }
            ViewBag.VehicleID = new SelectList(db.Vehicles, "VehicleID", "Code", trip.VehicleID);
            return View(trip);
        }

        // POST: Trips/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TripID,Name,VehicleID,Origin,Destination")] Trip trip)
        {
            if (ModelState.IsValid)
            {
                db.Entry(trip).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.VehicleID = new SelectList(db.Vehicles, "VehicleID", "Code", trip.VehicleID);
            return View(trip);
        }

        // GET: Trips/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Trip trip = db.Trips.Find(id);
            if (trip == null)
            {
                return HttpNotFound();
            }
            return View(trip);
        }

        // POST: Trips/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Trip trip = db.Trips.Find(id);
            db.Trips.Remove(trip);
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
