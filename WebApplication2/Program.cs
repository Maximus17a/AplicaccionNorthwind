using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using NorthwindSalesAnalysis.Models.Repository;
using NorthwindSalesAnalysis.Services;
using OfficeOpenXml;

var builder = WebApplication.CreateBuilder(args);

// Configurar la licencia de EPPlus para Excel
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

// Configurar DbContext
builder.Services.AddDbContext<NorthwindContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("NorthwindConnection")));

// Registrar servicios
builder.Services.AddScoped<INorthwindRepository, NorthwindRepository>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ExportService>();
builder.Services.AddHttpContextAccessor();

// Configurar autenticación
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(3);
    });

// Añadir servicios de MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure el pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();