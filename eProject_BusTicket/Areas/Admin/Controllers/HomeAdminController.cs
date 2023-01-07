using eProject_BusTicket.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace eProject_BusTicket.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class HomeAdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private int pageSize = 5;
        // GET: Admin/HomeAdmin
        public ActionResult Index(int? page)
        {
            int pageNumber = (page ?? 1);
            var bookings = db.Bookings.ToList();
            ViewBag.Earning = bookings.Sum(b => b.TotalPayment);
            ViewBag.Month = bookings.Where(b => b.DateTime.Month == DateTime.Now.Month)
                .Where(b=>b.DateTime.Year==DateTime.Now.Year)
                .Sum(b => b.TotalPayment);
            return View(bookings.ToPagedList(pageNumber, pageSize));
        }

        [HttpPost]
        public ActionResult Index(string search,int? page)
        {
            int pageNumber = (page ?? 1);
            ViewBag.Search = search;
            List<Booking> bl = new List<Booking>();
            if (search!=null)
            {
                var bookings = from b in db.Bookings
                    where b.Name.Contains(search) || b.PhoneNumber.Contains(search) || b.Email.Contains(search)
                    select b;
                bookings = bookings.OrderBy(b => b.Name);
                bl = bookings.ToList();
            }
            else
            {
                bl = db.Bookings.ToList();
            }
            
            return View("_Booking",bl.ToPagedList(pageNumber, pageSize));
        }

    }
}