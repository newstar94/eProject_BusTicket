using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection.Emit;
using System.Web;
using eProject_BusTicket.Models;

namespace eProject_BusTicket.Data
{
    public class AppDbContext:DbContext
    {
        public AppDbContext() : base("name=Conn")
        {
        }
        public DbSet<TypeofVehicle> TypeofVehicles { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Origin> Origins { get; set; }
        public DbSet<Destination> Destinations { get; set; }
        public DbSet<Trip> Trips { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingTicket> BookingsTickets { get; set; }
    }
}