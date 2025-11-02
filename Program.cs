using Bookify.Models;
using Bookify.Services;          // <-- si tu as créé OpenLibraryService / AmazonLinkBuilder
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Bookify.Data;
using Bookify.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --- DB (XAMPP/MariaDB) ---
// appsettings: "DefaultConnection": "Server=127.0.0.1;Port=3306;Database=bookmanagerdb;User=root;Password="
var cs = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDb>(opt =>
    opt.UseMySql(cs, ServerVersion.AutoDetect(cs)));   // ✅ AutoDetect (MariaDB/MySQL)

// --- MVC & API ---
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
    });

// Configure Entity Framework with MySQL
builder.Services.AddDbContext<ApplicationDb>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 33))
    ));

// --- Open Library + Amazon.com (optionnel, pour /api/book/{id}/amazon) ---
builder.Services.AddHttpClient<OpenLibraryService>(c =>
{
    c.Timeout = TimeSpan.FromSeconds(5);
    c.DefaultRequestHeaders.UserAgent.ParseAdd("Bookify/1.0");
});
builder.Services.AddSingleton(new AmazonLinkBuilder(defaultMarket: "com", affiliateTag: null));
// si tu as un tag affilié: new AmazonLinkBuilder("com", "tonid-20")
// Configure Entity Framework with MySQL
builder.Services.AddDbContext<ApplicationDb>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 33))
    ));

var app = builder.Build();

// --- Pipeline ---
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// --- Routing ---
// API attribuées (ex: /api/book, /api/book/{id}/amazon)
app.MapControllers();

// Vues MVC (si tu as HomeController/Views)
app.MapDefaultControllerRoute(); // équivaut à {controller=Home}/{action=Index}/{id?}

// Si tu veux absolument une route par défaut spécifique :
// app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Books}/{action=Index}/{id?}");

app.Run();
