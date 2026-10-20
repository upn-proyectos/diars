using Microsoft.EntityFrameworkCore;
using Minimarket.Datos.Context;

var builder = WebApplication.CreateBuilder(args);

// Configuración de Acceso a Datos (SQL Server con EF Core)
builder.Services.AddDbContext<MinimarketDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Servicios MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configuración del Pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
