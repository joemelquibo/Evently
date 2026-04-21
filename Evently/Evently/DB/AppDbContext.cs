using Microsoft.EntityFrameworkCore;
using Evently.Models;
using Microsoft.AspNetCore.Mvc;

namespace Evently.DB
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    }
}
