using System;
using System.Globalization;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using eProject_BusTicket.Data;
using eProject_BusTicket.Enum;
using eProject_BusTicket.Models;
using eProject_BusTicket.ViewModels;
using log4net;
namespace eProject_BusTicket.Controllers
{
    public class BookingsController : Controller
    {
        private AppDbContext db = new AppDbContext();

        private const string BookingSession = "BookingSession";
        //Cần tạo 1 session nữa lưu thông tin đăng nhập
        private const string BuyerSession = "BuyerSession";
        private static readonly ILog log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ActionResult Index()
        {
            var booking = db.Bookings;
            return View(booking.ToList());
        }

        // GET: Bookings
        public ActionResult Booking(int? id)

        {
            ViewBag.Name = "Dương"; //sau lấy id accout điền info của account vào
            ViewBag.PhoneNumber = "0123456789";
            ViewBag.Email = "duong@gmail.com";
            var BookingVM = new BookingVM();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RouteSchedule route = db.RouteSchedules.Find(id);
            TripSchedule trip = db.TripSchedules.Find(route.TripScheduleID);
            BookingVM.RouteDetails = route;
            BookingVM.TripDetails = trip;
            if (route == null)
            {
                return HttpNotFound();
            }
            return View(BookingVM);
        }

        [HttpPost]
        public ActionResult Booking(List<string> PassengerName, List<int> PassengerAge, int id, string Name, string Email, string PhoneNumber)
        {
            //var booking = Session[BookingSession];
            //var buyer = Session[BuyerSession];
            Buyer buyer = new Buyer();
            buyer.Email = Email;
            buyer.Name = Name;
            buyer.PhoneNumber = PhoneNumber;
            Session[BuyerSession] = buyer;
            RouteSchedule route = db.RouteSchedules.Find(id);
            List<BookingTicket> bookingTickets = new List<BookingTicket>();
            for (int i = 0; i < PassengerAge.Count; i++)
            {
                var ticket = new BookingTicket();
                ticket.RouteScheduleID = id;
                ticket.DepartureTime = route.DepartureTime;
                ticket.PassengerAge = PassengerAge[i];
                ticket.PassengerName = PassengerName[i];
                if (PassengerAge[i] < 6)
                {
                    ticket.Price = route.Route.Price * 0;
                }
                else if (PassengerAge[i] < 12)
                {
                    ticket.Price = route.Route.Price * (decimal)0.5;
                }
                else if (PassengerAge[i] > 50)
                {
                    ticket.Price = route.Route.Price * (decimal)0.7;
                }
                else
                {
                    ticket.Price = route.Route.Price * 1;
                }
                bookingTickets.Add(ticket);
            }

            Session[BookingSession] = bookingTickets;

            return RedirectToAction("Payment");
        }

        public ActionResult Payment()
        {
            var BookingVM = new List<BookingVM>();
            var bookingTickets = (List<BookingTicket>)Session[BookingSession];
            for (int i = 0; i < bookingTickets.Count; i++)
            {
                BookingVM booking = new BookingVM();
                var id = bookingTickets[i].RouteScheduleID;
                RouteSchedule route = db.RouteSchedules.Find(id);
                TripSchedule trip = db.TripSchedules.Find(route.TripScheduleID);
                booking.RouteDetails = route;
                booking.TripDetails = trip;
                booking.PassengerName = bookingTickets[i].PassengerName;
                booking.PassengerAge = bookingTickets[i].PassengerAge;
                booking.Price = bookingTickets[i].Price;
                BookingVM.Add(booking);
            }
            return View(BookingVM.ToList());
        }
        public ActionResult vnpay()
        {
            var bookingTickets = (List<BookingTicket>)Session[BookingSession];
            decimal TotalPayment = 0;
            foreach (var ticket in bookingTickets)
            {
                TotalPayment += ticket.Price;
            }
            string vnp_Returnurl = ConfigurationManager.AppSettings["vnp_Returnurl"]; //URL nhan ket qua tra ve 
            string vnp_Url = ConfigurationManager.AppSettings["vnp_Url"]; //URL thanh toan cua VNPAY 
            string vnp_TmnCode = ConfigurationManager.AppSettings["vnp_TmnCode"]; //Ma website
            string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Chuoi bi mat

            //Get payment input
            Booking booking = new Booking();
            //Save order to db
            string BookingCode = db.RouteSchedules.Find(bookingTickets[0].RouteScheduleID).RouteScheduleCode;
            booking.BookingCode = BookingCode + DateTime.Now.ToString("ddMMyyHHmmss"); // Giả lập mã giao dịch hệ thống merchant gửi sang VNPAY
            booking.TotalPayment = (int)TotalPayment; // Giả lập số tiền thanh toán hệ thống merchant gửi sang VNPAY 100,000 VND
            booking.Status = "0"; //0: Trạng thái thanh toán "chờ thanh toán" hoặc "Pending"
            booking.DateTime = DateTime.Now;
            //Build URL for VNPAY
            VnPayLibrary vnpay = new VnPayLibrary();

            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", (booking.TotalPayment * 100).ToString()); //Số tiền thanh toán. Số tiền không mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND(một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần(khử phần thập phân), sau đó gửi sang VNPAY là: 10000000

            vnpay.AddRequestData("vnp_CreateDate", booking.DateTime.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress());

            vnpay.AddRequestData("vnp_Locale", "vn");

            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang:" + booking.BookingCode);
            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", booking.BookingCode); // Mã tham chiếu của giao dịch tại hệ thống của merchant.Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY.Không đượctrùng lặp trong ngày
                                                                               //Add Params of 2.1.0 Version
            vnpay.AddRequestData("vnp_ExpireDate", DateTime.Now.AddMinutes(15).ToString("yyyyMMddHHmmss"));

            string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            log.InfoFormat("VNPAY URL: {0}", paymentUrl);
            return Redirect(paymentUrl);
        }

        public ActionResult Result()
        {
            var bookingTickets = (List<BookingTicket>)Session[BookingSession];
            var buyer =(Buyer) Session[BuyerSession];
            log.InfoFormat("Begin VNPAY Return, URL={0}", Request.RawUrl);
            if (Request.QueryString.Count > 0)
            {
                string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Chuoi bi mat
                var vnpayData = Request.QueryString;
                VnPayLibrary vnpay = new VnPayLibrary();

                foreach (string s in vnpayData)
                {
                    //get all querystring data
                    if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                    {
                        vnpay.AddResponseData(s, vnpayData[s]);
                    }
                }

                string BookingCode = Convert.ToString(vnpay.GetResponseData("vnp_TxnRef"));
                long TranId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
                string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
                string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
                String vnp_SecureHash = Request.QueryString["vnp_SecureHash"];
                long TotalPayment = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount")) / 100;
                bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);

                if (checkSignature)
                {
                    if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
                    {
                        //Tạo acc tesst
                        Account account = new Account();
                        account.Name = "Dương";
                        account.Email = "duong@gmail.com";
                        account.PhoneNumber = "0912345678";
                        account.Password = "1234";
                        account.Username = "newstar94";
                        account.Role = "Admin";
                        db.Accounts.Add(account);

                        //Thêm đơn booking
                        Booking booking = new Booking();
                        booking.Name = buyer.Name;
                        booking.Email = buyer.Email;
                        booking.PhoneNumber = buyer.PhoneNumber;
                        booking.TotalPayment = TotalPayment;
                        booking.BookingCode = BookingCode;
                        booking.DateTime = DateTime.Now;
                        booking.Status = "Success";
                        booking.TranId = TranId;
                        booking.AccountID = account.AccountID;
                        //thêm AccountID sau
                        db.Bookings.Add(booking);
                        db.SaveChanges();
                        //Trừ số ghế trống
                        var route = db.RouteSchedules.Find(bookingTickets[0].RouteScheduleID);
                        var routes = db.RouteSchedules.Where(r => r.TripScheduleID == route.TripScheduleID).ToList();
                        for (int i = 0; i < routes.Count; i++)
                        {  
                            if (routes[i].Route.EndID > route.Route.StartID && routes[i].Route.StartID<route.Route.EndID)
                            {
                                routes[i].AvaiableSeat -= bookingTickets.Count;
                                db.Entry(routes[i]).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                        //Lưu vé
                        foreach (var ticket in bookingTickets)
                        {
                            BookingTicket bookingTicket = new BookingTicket();
                            bookingTicket.BookingID = booking.BookingID;
                            bookingTicket.RouteScheduleID = ticket.RouteScheduleID;
                            bookingTicket.DepartureTime=ticket.DepartureTime;
                            bookingTicket.Price = ticket.Price;
                            bookingTicket.PassengerName = ticket.PassengerName;
                            bookingTicket.PassengerAge = ticket.PassengerAge;
                            bookingTicket.Status = TicketStatus.NotUsedYet;
                            db.BookingsTickets.Add(bookingTicket);
                            db.SaveChanges();
                        }

                        log.InfoFormat("Thanh toan thanh cong, OrderId={0}, VNPAY TranId={1}", BookingCode, TranId);
                    }
                    else
                    {
                        //Thanh toan khong thanh cong. Ma loi: vnp_ResponseCode

                        log.InfoFormat("Thanh toan loi, OrderId={0}, VNPAY TranId={1},ResponseCode={2}", BookingCode, TranId, vnp_ResponseCode);
                    }

                }
                else
                {
                    log.InfoFormat("Invalid signature, InputData={0}", Request.RawUrl);

                }
            }

            return View();
        }
    }

}
