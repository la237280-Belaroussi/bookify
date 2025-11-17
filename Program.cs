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

// --- CORS pour Angular (DEV) ---
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

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

// --- Swagger überall ---
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Bookify API v1");
    c.RoutePrefix = ""; // accès direct via https://localhost:7079/
});

// --- Middleware |
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// --- CORS doit être ici ---
app.UseCors();

app.UseAuthorization();

app.MapControllers();
app.MapDefaultControllerRoute();

app.Run();