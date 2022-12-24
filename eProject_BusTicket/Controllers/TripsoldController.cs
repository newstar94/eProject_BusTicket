using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;
using eProject_BusTicket.Data;
using eProject_BusTicket.Models;

namespace eProject_BusTicket.Controllers
{
    public class TripsController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: Trips
        public ActionResult Index()
        {
            var trips = db.Trips.Include(t => t.Destination).Include(t => t.Origin).Include(t => t.Vehicle);
            return View(trips.ToList());
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
            return View(trip);
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
            ViewBag.OriginAdd = new SelectList(db.Origins, "OriginID", "Address");
            ViewBag.DestinationAdd = new SelectList(db.Destinations, "DestinationID", "Address");
            ViewBag.DestinationID = new SelectList(db.Destinations, "DestinationID", "Name");
            ViewBag.OriginID = new SelectList(db.Origins, "OriginID", "Name");
            ViewBag.VehicleID = new SelectList(db.Vehicles, "VehicleID", "Code");
            ViewBag.TypeID = new SelectList(db.TypeofVehicles, "TypeID", "Name");
            return View();
        }

        // POST: Trips/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TypeID,TripID,Name,VehicleID,OriginID,DestinationID,DepartureTime,TripTime,Distance,Price,AvailableSeat")] Trip trip)
        {
            Vehicle vehicle = db.Vehicles.Find(trip.VehicleID);
            trip.Price = vehicle.Price * (decimal)trip.Distance;
            trip.AvailableSeat = vehicle.Seats;
            if (ModelState.IsValid)
            {
                db.Trips.Add(trip);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DestinationID = new SelectList(db.Destinations, "DestinationID", "Name", trip.DestinationID);
            ViewBag.OriginID = new SelectList(db.Origins, "OriginID", "Name", trip.OriginID);
            ViewBag.VehicleID = new SelectList(db.Vehicles, "VehicleID", "Code", trip.VehicleID);
            return View(trip);
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
            ViewBag.DestinationID = new SelectList(db.Destinations, "DestinationID", "Name", trip.DestinationID);
            ViewBag.OriginID = new SelectList(db.Origins, "OriginID", "Name", trip.OriginID);
            ViewBag.VehicleID = new SelectList(db.Vehicles, "VehicleID", "Code", trip.VehicleID);
            return View(trip);
        }

        // POST: Trips/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TripID,VehicleID,OriginID,DestinationID,DepartureTime,TripTime,Distance,Price,AvailableSeat")] Trip trip)
        {
            if (ModelState.IsValid)
            {
                db.Entry(trip).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DestinationID = new SelectList(db.Destinations, "DestinationID", "Name", trip.DestinationID);
            ViewBag.OriginID = new SelectList(db.Origins, "OriginID", "Name", trip.OriginID);
            ViewBag.VehicleID = new SelectList(db.Vehicles, "VehicleID", "Code", trip.VehicleID);
            return View(trip);
        }

        // POST: Trips/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
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
