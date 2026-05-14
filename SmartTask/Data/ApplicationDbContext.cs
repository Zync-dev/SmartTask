using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartTask.Models;

namespace SmartTask.Data
{
    public class ApplicationDbContext : IdentityDbContext // IdentityDbContext er fra ASP.NET Core Identity, så EF Core opretter automatisk brugere, logins, passwords osv.
    {
        // Her modtages instillingerne for databaseforbindelsen fra Program.cs
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) // base(options) sender indstillignerne videre til IdentityDbContext
        {
        }

        /* 
         * DbSet repræsenterer en tabel i databasen. EF Core opretter altså en tabel, der hedder TodoItems, ud fra klassen TodoItem.
         * Via denne property kan vi hente og gemme opgaver, fx _db.TodoItems.Where(...)
         */
        public DbSet<TodoItem> TodoItems { get; set; }
    }
}