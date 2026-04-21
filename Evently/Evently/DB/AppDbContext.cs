using Microsoft.EntityFrameworkCore;
using Evently.Models;
using Microsoft.AspNetCore.Mvc;

namespace Evently.DB
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<Attendances> Attendances { get; set; }
        public DbSet<Events> Events { get; set; }
        public DbSet<Payments> Payments { get; set; }
        public DbSet<Registrations> Registrations { get; set; }
    }
}
