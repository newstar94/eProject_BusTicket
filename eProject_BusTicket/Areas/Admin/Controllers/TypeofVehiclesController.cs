using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using eProject_BusTicket.Data;

namespace eProject_BusTicket.Areas.Admin.Controllers
{
    public class TypeofVehiclesController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: TypeofVehicles
        public ActionResult Index()
        {
            return View(db.TypeofVehicles.ToList());
        }

        // GET: TypeofVehicles/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TypeofVehicles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TypeID,Name")] TypeofVehicle typeofVehicle)
        {
            if (ModelState.IsValid)
            {
                db.TypeofVehicles.Add(typeofVehicle);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(typeofVehicle);
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
