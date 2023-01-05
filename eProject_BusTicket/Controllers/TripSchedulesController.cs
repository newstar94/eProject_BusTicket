using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using eProject_BusTicket.Models;
using eProject_BusTicket.ViewModels;

namespace eProject_BusTicket.Controllers
{
    [AllowAnonymous]
    public class TripSchedulesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: TripSchedules
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
                .Where(t => t.Trip.Origin == origin)
                .Where(t => t.Trip.Destination == destination)
                .Where(t => t.Date == dateTime)
                .Include(t => t.Trip);
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