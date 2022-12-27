using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using eProject_BusTicket.Data;
using eProject_BusTicket.Enum;
using eProject_BusTicket.Models;

namespace eProject_BusTicket.Controllers
{
    public class BookingTicketsController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: BookingTickets
        public ActionResult Index(int? id)
        {
            var Tickets = db.BookingsTickets.Where(t=>t.BookingID==id).Include(b => b.Booking).Include(b => b.RouteSchedule).ToList();
            foreach (var ticket in Tickets)
            {
                if (ticket.DepartureTime < DateTime.Now && ticket.Status == TicketStatus.NotUsedYet)
                {
                    ticket.Status = TicketStatus.Used;
                    db.Entry(ticket).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return View(Tickets.ToList());
        }

        public ActionResult Cancel(int? id)
        {
            var vnpayApiUrl = ConfigurationManager.AppSettings["querydr"];
            var vnpHashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"];
            var vnpTmnCode = ConfigurationManager.AppSettings["vnp_TmnCode"];
            var vnpay = new VnPayLibrary();
            var createDate = DateTime.Now;

            var ticket = db.BookingsTickets.Find(id);
            var booking = db.Bookings.Find(ticket.BookingID);


            try
            {
                var amountrf = 0;
                if (DateTime.Now.AddDays(2) <= ticket.DepartureTime)
                {
                    amountrf = Convert.ToInt32(ticket.Price);
                }
                else if (DateTime.Now.AddDays(1) <= ticket.DepartureTime)
                {
                    amountrf = Convert.ToInt32(ticket.Price * (decimal)0.85);
                }
                else
                {
                    amountrf = Convert.ToInt32(ticket.Price * (decimal)0.7);
                }

                vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
                vnpay.AddRequestData("vnp_Command", "refund");
                vnpay.AddRequestData("vnp_TmnCode", vnpTmnCode);

                if (amountrf==ticket.Price)
                {
                    vnpay.AddRequestData("vnp_TransactionType", "03");
                }
                else
                {
                    vnpay.AddRequestData("vnp_TransactionType", "03");
                }
                
                vnpay.AddRequestData("vnp_CreateBy", booking.Email);
                vnpay.AddRequestData("vnp_TxnRef", booking.BookingCode);
                vnpay.AddRequestData("vnp_Amount", (amountrf*100).ToString());
                vnpay.AddRequestData("vnp_OrderInfo", "REFUND ORDERID:" + booking.BookingCode);
                vnpay.AddRequestData("vnp_TransDate", booking.DateTime.ToString("yyyyMMddHHmmss"));
                vnpay.AddRequestData("vnp_CreateDate", createDate.ToString("yyyyMMddHHmmss"));
                vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress());

                var strDatax = "";

                var refundtUrl = vnpay.CreateRequestUrl(vnpayApiUrl, vnpHashSecret);

                var request = (HttpWebRequest)WebRequest.Create(refundtUrl);
                request.AutomaticDecompression = DecompressionMethods.GZip;
                using (var response = (HttpWebResponse)request.GetResponse())
                using (var stream = response.GetResponseStream())
                    if (stream != null)
                        using (var reader = new StreamReader(stream))
                        {
                            strDatax = reader.ReadToEnd();
                        }
                var data = strDatax.Split('&');
                var vnpResponseCode = data[5].Split('=');
                var responsecode = vnpResponseCode[1];
                if (responsecode == "00")
                {
                    ticket.Status = TicketStatus.Canceled;
                    ticket.Price -= amountrf;
                    db.Entry(ticket).State = EntityState.Modified;
                    db.SaveChanges();
                    //Cộng lại số ghế trống
                    var route = db.RouteSchedules.Find(ticket.RouteScheduleID);
                    var routes = db.RouteSchedules.Where(r => r.TripScheduleID == route.TripScheduleID).ToList();
                    for (int i = 0; i < routes.Count; i++)
                    {
                        if (routes[i].Route.EndID > route.Route.StartID && routes[i].Route.StartID < route.Route.EndID)
                        {
                            routes[i].AvaiableSeat += 1;
                            db.Entry(routes[i]).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    db.Entry(route).State = EntityState.Modified;
                    db.SaveChanges();
                    booking.TotalPayment -= amountrf;
                    db.Entry(booking).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Result = "Có lỗi sảy ra trong quá trình hoàn tiền:" + ex;
            }

            return View();
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
