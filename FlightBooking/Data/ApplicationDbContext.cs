using FlightBooking.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FlightBooking.Data
{
    public class ApplicationDbContext:IdentityDbContext
    {

        public ApplicationDbContext(DbContextOptions options) : base(options)
        {



        }

        public DbSet<AppUser> AppUser { get; set; }
        public DbSet<Flight> Flight { get; set; }

    }
}
