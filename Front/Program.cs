using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Minimarket.Datos.Context;
using Minimarket.Datos.Repositories;
using Minimarket.Negocio.Interfaces;
using Minimarket.Negocio.Patterns.Adapter;
using Minimarket.Negocio.Patterns.Singleton;
using Minimarket.Negocio.Patterns.Strategy;
using Minimarket.Negocio.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Configuración de Acceso a Datos (SQL Server con EF Core)
builder.Services.AddDbContext<MinimarketDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Inyección de Dependencias (Patrón Repository + Patrón Adapter + Patrón Singleton + Patrón Strategy + Servicios)
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
builder.Services.AddScoped<IProveedorRepository, ProveedorRepository>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IVentaRepository, VentaRepository>();
builder.Services.AddScoped<IOrdenCompraRepository, OrdenCompraRepository>();
builder.Services.AddScoped<IKardexRepository, KardexRepository>();

builder.Services.AddSingleton<SunatReniecMotorExterno>();
builder.Services.AddSingleton(_ => ConfiguracionTiendaSingleton.Instancia);
builder.Services.AddScoped<PagoStrategyContext>();
builder.Services.AddScoped<IValidadorDocumentoAdapter, ValidadorDocumentoPeruAdapter>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<IProveedorService, ProveedorService>();
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IVentaService, VentaService>();
builder.Services.AddScoped<IOrdenCompraService, OrdenCompraService>();
builder.Services.AddScoped<IKardexService, KardexService>();

// 3. Autenticación Basada en Cookies para Sesión Web Segura
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

// 4. Servicios MVC
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

// Orden crítico de seguridad en ASP.NET Core: Authentication antes de Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
