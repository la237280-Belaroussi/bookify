using Bookify.Data;
using Bookify.Models;
using Bookify.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// --- Database ---
var cs = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDBContext>(opt =>
    opt.UseMySql(cs, ServerVersion.AutoDetect(cs)));

// --- MVC Controllers + Views ---
builder.Services.AddControllersWithViews();

// --- Services OpenLibrary et Amazon ---
builder.Services.AddHttpClient<OpenLibraryService>(c =>
{
    c.Timeout = TimeSpan.FromSeconds(5);
    c.DefaultRequestHeaders.UserAgent.ParseAdd("Bookify/1.0");
});
builder.Services.AddSingleton(new AmazonLinkBuilder(defaultMarket: "com", affiliateTag: null));
builder.Services.AddScoped<AuthorizationService>();

// --- Swagger ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Bookify API",
        Version = "v1",
        Description = "API REST pour la gestion des livres et des genres"
    });
});

var app = builder.Build();

// --- Pipeline ---
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// --- Swagger (accessible dans tous les environnements) ---
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Bookify API v1");
    c.RoutePrefix = ""; // Swagger directement sur la racine http://localhost:5211/
});

// --- Middleware ---
app.UseHttpsRedirection();
app.UseStaticFiles();

// -- Routing & Authorization ---
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

// Route par défaut MVC (Controllers + Views)
app.MapDefaultControllerRoute();

app.Run();
