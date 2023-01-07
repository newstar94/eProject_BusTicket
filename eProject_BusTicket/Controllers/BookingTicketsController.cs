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

namespace eProject_BusTicket.Controllers
{
    [Authorize(Roles = "User")]
    public class BookingTicketsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BookingTickets
        public ActionResult Index(int? id)
        {
            List<BookingTicket> tickets = new List<BookingTicket>();

            if (TempData["tickets"] == null)
            {
                if (id == null)
                {
                    var userId = User.Identity.GetUserId();
                    var bookings = db.Bookings.Where(b => b.UserID == userId).ToList();
                    var ticketlist = db.BookingsTickets.ToList();
                    foreach (var booking in bookings)
                    {
                        tickets.AddRange(ticketlist.Where(t => t.BookingID == booking.BookingID).ToList());
                    }
                }
                else
                {
                    tickets = db.BookingsTickets.Where(t => t.BookingID == id)
                        .Include(b => b.Booking).Include(b => b.RouteSchedule).ToList();
                }

                foreach (var ticket in tickets)
                {
                    if (ticket.DepartureTime < DateTime.Now && ticket.Status == TicketStatus.NotUsedYet)
                    {
                        ticket.Status = TicketStatus.Used;
                        db.Entry(ticket).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
            else
            {
                tickets = (List<BookingTicket>)TempData["tickets"];
            }
            return View(tickets);
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

                if (amountrf == ticket.Price)
                {
                    vnpay.AddRequestData("vnp_TransactionType", "03");
                }
                else
                {
                    vnpay.AddRequestData("vnp_TransactionType", "03");
                }

                vnpay.AddRequestData("vnp_CreateBy", booking.Email);
                vnpay.AddRequestData("vnp_TxnRef", booking.BookingCode);
                vnpay.AddRequestData("vnp_Amount", (amountrf * 100).ToString());
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
                    TempData["Cancel"] = "Cancel success! Money will be refunded within 5 working days";
                    booking.TotalPayment -= amountrf;
                    db.Entry(booking).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                TempData["Cancel"] = "Have an error: " + ex;
            }

            return RedirectToAction("Index", new { id = booking.BookingID });
        }


        public ActionResult Print(int? id)
        {

            var ticket = db.BookingsTickets.Find(id);
            var start = ticket.RouteSchedule.Route.Start;
            var end = ticket.RouteSchedule.Route.End;
            var name = ticket.PassengerName;
            var age = ticket.PassengerAge;
            var code = ticket.TicketCode;
            var departuretime = ticket.DepartureTime.ToString("HH:mm dd/MM/yyyy");


            Document pdfDoc = new Document(new Rectangle(796, 324),50,10,90,10);
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
            pdfDoc.Open();
            BaseFont baseFont = BaseFont.CreateFont("c:/windows/fonts/arial.ttf", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            Font font = new Font(baseFont,14,Font.NORMAL,BaseColor.BLACK);



            //set background
            Image image = Image.GetInstance(Server.MapPath("~/Images/Ticket.png"));
            image.Alignment = Image.UNDERLYING;
            image.SetAbsolutePosition(0, 0);
            pdfDoc.Add(image);

            Paragraph paragraph = new Paragraph("Ticket code:" + code, FontFactory.GetFont("Arial", 14, Font.BOLD, BaseColor.BLACK));
            paragraph.Alignment = Element.ALIGN_RIGHT;
            pdfDoc.Add(paragraph);

            //set ticket
            PdfPTable table = new PdfPTable(7);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 10f;
            table.SpacingAfter = 10f;
            
            PdfPCell cell = new PdfPCell();
            paragraph = new Paragraph("Start:",FontFactory.GetFont("Arial",14,Font.BOLD,BaseColor.BLACK));
            cell.AddElement(paragraph);
            cell.Border = 0;
            table.AddCell(cell);
            cell = new PdfPCell();
            paragraph = new Paragraph(start, font);
            cell.Colspan = 6;
            cell.Border = 0;
            cell.AddElement(paragraph);
            table.AddCell(cell);


            cell = new PdfPCell();
            paragraph = new Paragraph("End:", FontFactory.GetFont("Arial", 14, Font.BOLD, BaseColor.BLACK));
            cell.AddElement(paragraph);
            cell.Border = 0;
            table.AddCell(cell);
            cell = new PdfPCell();
            paragraph = new Paragraph(end, font);
            cell.Colspan = 6;
            cell.Border = 0;
            cell.AddElement(paragraph);
            table.AddCell(cell);

            cell = new PdfPCell();
            paragraph = new Paragraph("Time:", FontFactory.GetFont("Arial", 14, Font.BOLD, BaseColor.BLACK));
            cell.AddElement(paragraph);
            cell.Border = 0;
            table.AddCell(cell);
            cell = new PdfPCell();
            paragraph = new Paragraph(departuretime, font);
            cell.Colspan = 6;
            cell.Border = 0;
            cell.AddElement(paragraph);
            table.AddCell(cell);

            cell = new PdfPCell();
            paragraph = new Paragraph("Name:", FontFactory.GetFont("Arial", 14, Font.BOLD, BaseColor.BLACK));
            cell.AddElement(paragraph);
            cell.Border = 0;
            table.AddCell(cell);
            cell = new PdfPCell();
            paragraph = new Paragraph(name, font);
            cell.Colspan = 6;
            cell.Border = 0;
            cell.AddElement(paragraph);
            table.AddCell(cell);

            cell = new PdfPCell();
            paragraph = new Paragraph("Age:", FontFactory.GetFont("Arial", 14, Font.BOLD, BaseColor.BLACK));
            cell.AddElement(paragraph);
            cell.Border = 0;
            table.AddCell(cell);
            cell = new PdfPCell();
            paragraph = new Paragraph(age.ToString(), font);
            cell.Colspan = 6;
            cell.Border = 0;
            cell.AddElement(paragraph);
            table.AddCell(cell);

            pdfDoc.Add(table);

            pdfWriter.CloseStream = false;
            pdfDoc.Close();
            Response.Buffer = true;
            Response.ContentType = "application/pdf";
            string filename = "Ticket " + code + ".pdf";
            Response.AddHeader("content-disposition", "attachment;filename=" + filename);
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(pdfDoc);
            Response.End();

            return null;
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