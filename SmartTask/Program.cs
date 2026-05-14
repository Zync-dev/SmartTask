using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartTask.Data;
using SmartTask.Services;

var builder = WebApplication.CreateBuilder(args);

// Tilføj DbContext med MySQL - Henter instillinger fra appsettings.json.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection")!,
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection")!)
    ));

// Tilføj Identity - indstillinger, f.eks. passwordkrav.
// .AddEntityFrameworkStores<ApplicationDbContext>(); fortæller, at brugere skal gemmes i databasen
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<ApplicationDbContext>();

// Registrerer AI-serviven, så den kan bruges til dependency-injection
builder.Services.AddScoped<GroqService>();

// Tilføj Razor Pages
builder.Services.AddRazorPages();

var app = builder.Build();

// Kører automatisk databasemigrationer, hvis der skulle være manglende migrationer
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

app.UseStaticFiles(); // Tillad CSS, billeder osv.
app.UseRouting(); // Aktiver URL-routing
app.UseAuthentication(); // Tjek om brugeren er logget ind
app.UseAuthorization(); // Tjek om brugeren har adgang
app.MapRazorPages(); // Forbind URLs til Razor Pages

app.Run(); // Starter serveren