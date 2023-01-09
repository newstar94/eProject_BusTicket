using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace eProject_BusTicket.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("Connection", throwIfV1Schema: false)
        {
        }

        static ApplicationDbContext()
        {
            // Set the database initializer which is run once during application start
            // This seeds the database with admin user credentials and admin role
            Database.SetInitializer<ApplicationDbContext>(new ApplicationDbInitializer());
        }

        public DbSet<TypeofVehicle> TypeofVehicles { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Trip> Trips { get; set; }
        public DbSet<TripSchedule> TripSchedules { get; set; }
        public DbSet<Route> Routes { get; set; }
        public DbSet<RouteSchedule> RouteSchedules { get; set; }
        public DbSet<Station> Stations { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingTicket> BookingsTickets { get; set; }
        public DbSet<TimeUsed> TimeUseds { get; set; }
        
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}