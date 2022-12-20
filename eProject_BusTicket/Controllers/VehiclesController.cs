using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using eProject_BusTicket.Data;

namespace eProject_BusTicket.Controllers
{
    public class VehiclesController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: Vehicles
        public ActionResult Index()
        {
            var vehicles = db.Vehicles.Include(v => v.TypeofVehicle);
            return View(vehicles.ToList());
        }

        // GET: Vehicles/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vehicle vehicle = db.Vehicles.Find(id);
            if (vehicle == null)
            {
                return HttpNotFound();
            }
            return View(vehicle);
        }

        // GET: Vehicles/Create
        public ActionResult Create()
        {

            ViewBag.TypeID = new SelectList(db.TypeofVehicles, "TypeID", "Name");
            return View();
        }

        // POST: Vehicles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "VehicleID,Seats,Price,Code,TypeID")] Vehicle vehicle)
        {
            ViewBag.TypeID = new SelectList(db.TypeofVehicles, "TypeID", "Name", vehicle.TypeID);
            if (ModelState.IsValid)
            {
                TypeofVehicle typeofVehicle = db.TypeofVehicles.Find(vehicle.TypeID);
                vehicle.Code = typeofVehicle.Name.Substring(0,2).ToUpper() + new Random().Next(100,999).ToString();
                db.Vehicles.Add(vehicle);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(vehicle);
        }

        // GET: Vehicles/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vehicle vehicle = db.Vehicles.Find(id);
            if (vehicle == null)
            {
                return HttpNotFound();
            }
            ViewBag.TypeID = new SelectList(db.TypeofVehicles, "TypeID", "Name", vehicle.TypeID);
            return View(vehicle);
        }

        // POST: Vehicles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "VehicleID,Seats,Price,Code,TypeID")] int? id, Vehicle Modifiedvehicle)
        {
            Vehicle vehicle = db.Vehicles.Find(id);
            vehicle.Code = vehicle.Code;
            vehicle.Seats = vehicle.Seats;
            vehicle.TypeID=vehicle.TypeID;
            vehicle.Price= Modifiedvehicle.Price;
            if (ModelState.IsValid)
            {
                db.Entry(vehicle).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TypeID = new SelectList(db.TypeofVehicles, "TypeID", "Name", vehicle.TypeID);
            return View(vehicle);
        }

        // Delete: Vehicles/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            Vehicle vehicle = db.Vehicles.Find(id);
            db.Vehicles.Remove(vehicle);
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
