using Microsoft.EntityFrameworkCore;
using Minimarket.Datos.Context;
using Minimarket.Datos.Repositories;
using Minimarket.Negocio.Interfaces;
using Minimarket.Negocio.Patterns.Adapter;
using Minimarket.Negocio.Patterns.Singleton;
using Minimarket.Negocio.Patterns.Strategy;
using Minimarket.Negocio.Services;

var builder = WebApplication.CreateBuilder(args);

// Configuración de Base de Datos SQL Server con EF Core
builder.Services.AddDbContext<MinimarketDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Inyección de Dependencias: Patrón Repository, Patrón Adapter, Patrón Singleton, Patrón Strategy y Servicios de Negocio
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
builder.Services.AddScoped<IProveedorRepository, ProveedorRepository>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IVentaRepository, VentaRepository>();

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

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Configuración de CORS para permitir consumo desde el Frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();
