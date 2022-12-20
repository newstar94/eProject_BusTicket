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
    public class BookingsController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: Bookings
        public ActionResult Index(int id)

        {
            var BookingVM = new BookingVM();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Trip trip = db.Trips.Find(id);
            BookingVM.TripDetails=trip;
            if (trip == null)
            {
                return HttpNotFound();
            }
            return View(BookingVM);
        }

        [HttpPost]
        public ActionResult Index(List<string> Name, int id)
        {
            Trip trip = db.Trips.Find(id);
            return View();
        }
    }
    
}
