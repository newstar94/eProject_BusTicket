using eProject_BusTicket.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eProject_BusTicket.ViewModes;
using eProject_BusTicket.Data;

namespace eProject_BusTicket.Controllers
{

    public class LocationsController : Controller
    {
        private AppDbContext db = new AppDbContext();
        // GET: Locations
        public ActionResult Index()
        {
            var origins = db.Origins;
            return View(origins.ToList());
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Location location)
        {
            Origin origin = new Origin();
            Destination destination = new Destination();
            origin.OriginID = location.LocationID;
            origin.Name = location.Name;

            destination.DestinationID = location.LocationID;
            destination.Name = location.Name;

            if (ModelState.IsValid)
            {
                db.Destinations.Add(destination);
                db.SaveChanges();
                db.Origins.Add(origin);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
       
    }
}