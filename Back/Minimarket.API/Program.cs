using Microsoft.EntityFrameworkCore;
using Minimarket.Datos.Context;
using Minimarket.Negocio.Interfaces;
using Minimarket.Negocio.Services;

var builder = WebApplication.CreateBuilder(args);

// Configuración de Base de Datos SQL Server con EF Core
builder.Services.AddDbContext<MinimarketDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Inyección de Dependencias para Servicios de Negocio
builder.Services.AddScoped<IAuthService, AuthService>();

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
