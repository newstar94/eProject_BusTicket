using System;
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
using eProject_BusTicket.Models;
using eProject_BusTicket.ViewModels;
using log4net;
namespace eProject_BusTicket.Controllers
{
    public class BookingsController : Controller
    {
        private AppDbContext db = new AppDbContext();

        private const string BookingSession = "BookingSession";
        private static readonly ILog log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // GET: Bookings
        public ActionResult Booking(int? id)

        {
            var BookingVM = new BookingVM();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RouteSchedule route = db.RouteSchedules.Find(id);
            TripSchedule trip = db.TripSchedules.Find(route.TripScheduleID);
            BookingVM.RouteDetails = route;
            BookingVM.TripDetails=trip;
            if (route == null)
            {
                return HttpNotFound();
            }
            return View(BookingVM);
        }

        [HttpPost]
        public ActionResult Booking(List<string> PassengerName, List<int> PassengerAge, int id)
        {
            var booking = Session[BookingSession];
            RouteSchedule route = db.RouteSchedules.Find(id);
            List<BookingTicket> bookingTickets = new List<BookingTicket>();
            for (int i = 0; i < PassengerAge.Count; i++)
            {
                var ticket = new BookingTicket();
                ticket.RouteScheduleID = id;
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
            var id = 0;
            var BookingVM = new List<BookingVM>();
            var bookingTickets = (List<BookingTicket>)Session[BookingSession];
            for (int i = 0; i < bookingTickets.Count; i++)
            {
                BookingVM booking = new BookingVM();
                id = bookingTickets[i].RouteScheduleID;
                RouteSchedule route = db.RouteSchedules.Find(id);
                TripSchedule trip = db.TripSchedules.Find(route.TripScheduleID);
                booking.RouteDetails = route;
                booking.TripDetails=trip;
                booking.PassengerName = bookingTickets[i].PassengerName;
                booking.PassengerAge = bookingTickets[i].PassengerAge;
                booking.Price = bookingTickets[i].Price;
                BookingVM.Add(booking);
            }
            return View(BookingVM.ToList());
        }
        public ActionResult vnpay()
        {
            string vnp_Returnurl = ConfigurationManager.AppSettings["vnp_Returnurl"]; //URL nhan ket qua tra ve 
            string vnp_Url = ConfigurationManager.AppSettings["vnp_Url"]; //URL thanh toan cua VNPAY 
            string vnp_TmnCode = ConfigurationManager.AppSettings["vnp_TmnCode"]; //Ma website
            string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Chuoi bi mat

            //Get payment input
            Booking order = new Booking();
            //Save order to db
            order.BookingID = (int)DateTime.Now.Ticks; // Giả lập mã giao dịch hệ thống merchant gửi sang VNPAY
            order.TotalPayment = 100000; // Giả lập số tiền thanh toán hệ thống merchant gửi sang VNPAY 100,000 VND
            order.Status = "0"; //0: Trạng thái thanh toán "chờ thanh toán" hoặc "Pending"
            order.BookingCode = "txtOrderDesc.Text";
            order.DateTime = DateTime.Now;
            //Build URL for VNPAY
            VnPayLibrary vnpay = new VnPayLibrary();

            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", (order.TotalPayment * 100).ToString()); //Số tiền thanh toán. Số tiền không mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND(một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần(khử phần thập phân), sau đó gửi sang VNPAY là: 10000000

            vnpay.AddRequestData("vnp_CreateDate", order.DateTime.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress());

            vnpay.AddRequestData("vnp_Locale", "vn");

            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang:" + order.BookingID);
            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", order.BookingID.ToString()); // Mã tham chiếu của giao dịch tại hệ thống của merchant.Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY.Không đượctrùng lặp trong ngày
                                                                            //Add Params of 2.1.0 Version
            vnpay.AddRequestData("vnp_ExpireDate", DateTime.Now.AddMinutes(15).ToString("yyyyMMddHHmmss"));

            string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            log.InfoFormat("VNPAY URL: {0}", paymentUrl);
            return Redirect(paymentUrl);
        }
        
        public ActionResult Result()
        {
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

                long orderId = Convert.ToInt64(vnpay.GetResponseData("vnp_TxnRef"));
                long vnpayTranId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
                string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
                string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
                String vnp_SecureHash = Request.QueryString["vnp_SecureHash"];
                String TerminalID = Request.QueryString["vnp_TmnCode"];
                long vnp_Amount = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount")) / 100;
                String bankCode = Request.QueryString["vnp_BankCode"];

                bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);
                if (checkSignature)
                {
                    if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
                    {
                        //Thanh toan thanh cong
                       
                        log.InfoFormat("Thanh toan thanh cong, OrderId={0}, VNPAY TranId={1}", orderId, vnpayTranId);
                    }
                    else
                    {
                        //Thanh toan khong thanh cong. Ma loi: vnp_ResponseCode
                       
                        log.InfoFormat("Thanh toan loi, OrderId={0}, VNPAY TranId={1},ResponseCode={2}", orderId, vnpayTranId, vnp_ResponseCode);
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
