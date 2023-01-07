using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using eProject_BusTicket.Areas.Admin.Enum;
using eProject_BusTicket.Models;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;

namespace eProject_BusTicket.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BookingTicketsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BookingTickets
        public ActionResult Index(int? id)
        {
            List<BookingTicket> tickets = new List<BookingTicket>();
            tickets = db.BookingsTickets.Where(t => t.BookingID == id || id == null)
                        .Include(b => b.Booking).Include(b => b.RouteSchedule).ToList();
            foreach (var ticket in tickets)
            {
                if (ticket.DepartureTime < DateTime.Now && ticket.Status == TicketStatus.NotUsedYet)
                {
                    ticket.Status = TicketStatus.Used;
                    db.Entry(ticket).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return View(tickets);
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