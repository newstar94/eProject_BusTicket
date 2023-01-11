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
    public class TypeofVehiclesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// GET: TypeofVehicles
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
            var check = true;
            var types = db.TypeofVehicles.ToList();
            foreach (var type in types)
            {
                if (type.Name.ToLower() == typeofVehicle.Name.ToLower())
                {
                    ModelState.AddModelError("", "Type of Vehicle has exist!");
                    check = false;
                    break;
                }
            }
            if (ModelState.IsValid && check == true)
            {
                typeofVehicle.IsActive = true;
                db.TypeofVehicles.Add(typeofVehicle);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(typeofVehicle);
        }

        [HttpPost]
        public JsonResult ChangeStatus(int id)
        {
            var type = db.TypeofVehicles.Find(id);
            type.IsActive = !type.IsActive;
            db.Entry(type).State = EntityState.Modified;
            db.SaveChanges();
            return Json(type.IsActive);
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