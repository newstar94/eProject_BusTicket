using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eProject_BusTicket.Models;

namespace eProject_BusTicket.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class LocationsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// GET: Locations
        public ActionResult Index()
        {
            return View(db.Locations.ToList());
        }

        public ActionResult Create()
        {
            return View();
        }

        // POST: Locations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LocationID,LocationName")] Location location)
        {
            var check = true;
            var locations = db.Locations.ToList();
            foreach (var lo in locations)
            {
                if (location.LocationName.ToLower() == lo.LocationName.ToLower())
                {
                    ModelState.AddModelError("", "Location has exist!");
                    check = false;
                    break;
                }
            }
            if (ModelState.IsValid && check == true)
            {
                location.IsActive = true;
                db.Locations.Add(location);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(location);
        }

        [HttpPost]
        public JsonResult ChangeStatus(int id)
        {
            var location = db.Locations.Find(id);
            location.IsActive = !location.IsActive;
            db.Entry(location).State = EntityState.Modified;
            db.SaveChanges();

            return Json(location.IsActive);
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