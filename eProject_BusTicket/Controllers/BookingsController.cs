using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using eProject_BusTicket.Data;
using eProject_BusTicket.Models;
using eProject_BusTicket.ViewModels;

namespace eProject_BusTicket.Controllers
{
    public class BookingsController : Controller
    {
        private AppDbContext db = new AppDbContext();

        private const string BookingSession = "BookingSession";
        // GET: Bookings
        public ActionResult Booking(int? id)

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
        public ActionResult Booking(List<string> PassengerName, List<int> PassengerAge, int id)
        {
            var booking = Session[BookingSession];
            Trip trip = db.Trips.Find(id);
            List<BookingTicket> bookingTickets = new List<BookingTicket>();
            for (int i = 0; i < PassengerAge.Count; i++)
            {
                var ticket = new BookingTicket();
                ticket.TripID = id;
                ticket.PassengerAge = PassengerAge[i];
                ticket.PassengerName = PassengerName[i];
                if (PassengerAge[i] < 6)
                {
                    ticket.Price = trip.Price * 0;
                }
                else if (PassengerAge[i] < 12)
                {
                    ticket.Price = trip.Price * (decimal)0.5;
                }
                else if (PassengerAge[i] > 50)
                {
                    ticket.Price = trip.Price * (decimal)0.7;
                }
                else
                {
                    ticket.Price = trip.Price * 1;
                }
                bookingTickets.Add(ticket);
            }

            Session[BookingSession] = bookingTickets;

            return RedirectToAction("Payment");
        }

        public ActionResult Payment()
        {
            var id = 0;
            var BookingVM = new List<BookingVM>();
            var bookingTickets = (List < BookingTicket >)Session[BookingSession];
            for (int i = 0; i < bookingTickets.Count; i++)
            {
                BookingVM booking = new BookingVM();
                id = bookingTickets[i].TripID;
                Trip trip = db.Trips.Find(id);
                booking.TripDetails = trip;
                booking.PassengerName = bookingTickets[i].PassengerName;
                booking.PassengerAge = bookingTickets[i].PassengerAge;
                booking.Price = bookingTickets[i].Price;
                BookingVM.Add(booking);
            }
            return View(BookingVM.ToList());
        }
    }
    
}
