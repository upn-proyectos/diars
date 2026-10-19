using Microsoft.EntityFrameworkCore;
using Minimarket.Datos.Entities;

namespace Minimarket.Datos.Context;

public class MinimarketDbContext : DbContext
{
    public MinimarketDbContext(DbContextOptions<MinimarketDbContext> options) : base(options)
    {
    }

    public DbSet<Rol> Roles => Set<Rol>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Proveedor> Proveedores => Set<Proveedor>();
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Venta> Ventas => Set<Venta>();
    public DbSet<DetalleVenta> DetalleVentas => Set<DetalleVenta>();
    public DbSet<Kardex> Kardex => Set<Kardex>();
    public DbSet<ConfiguracionTienda> Configuracion => Set<ConfiguracionTienda>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Roles
        modelBuilder.Entity<Rol>(entity =>
        {
            entity.ToTable("Roles");
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Nombre).HasMaxLength(50).IsRequired();
            entity.HasIndex(r => r.Nombre).IsUnique();
        });

        // Usuarios
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("Usuarios");
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Nombre).HasMaxLength(150).IsRequired();
            entity.Property(u => u.UsuarioLogin).HasColumnName("Usuario").HasMaxLength(50).IsRequired();
            entity.HasIndex(u => u.UsuarioLogin).IsUnique();
            entity.Property(u => u.PasswordHash).HasMaxLength(255).IsRequired();

            entity.HasOne(u => u.Rol)
                  .WithMany(r => r.Usuarios)
                  .HasForeignKey(u => u.RolId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Categorias
        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.ToTable("Categorias");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Nombre).HasMaxLength(100).IsRequired();
            entity.HasIndex(c => c.Nombre).IsUnique();
        });

        // Proveedores
        modelBuilder.Entity<Proveedor>(entity =>
        {
            entity.ToTable("Proveedores");
            entity.HasKey(pr => pr.Id);
            entity.Property(pr => pr.Ruc).HasMaxLength(20).IsRequired();
            entity.HasIndex(pr => pr.Ruc).IsUnique();
            entity.Property(pr => pr.RazonSocial).HasMaxLength(150).IsRequired();
            entity.Property(pr => pr.NombreComercial).HasMaxLength(150);
            entity.Property(pr => pr.Contacto).HasMaxLength(100);
            entity.Property(pr => pr.Telefono).HasMaxLength(30);
            entity.Property(pr => pr.Email).HasMaxLength(100);
            entity.Property(pr => pr.Direccion).HasMaxLength(200);
        });

        // Productos
        modelBuilder.Entity<Producto>(entity =>
        {
            entity.ToTable("Productos");
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Nombre).HasMaxLength(150).IsRequired();
            entity.Property(p => p.PrecioVenta).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(p => p.CostoUnitario).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(p => p.Stock).HasColumnType("decimal(10,3)").IsRequired();
            entity.Property(p => p.StockMinimo).HasColumnType("decimal(10,3)").IsRequired();
            entity.Property(p => p.UnidadMedida).HasMaxLength(20).HasDefaultValue("UNIDAD").IsRequired();

            entity.HasOne(p => p.Categoria)
                  .WithMany(c => c.Productos)
                  .HasForeignKey(p => p.CategoriaId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(p => p.Proveedor)
                  .WithMany(pr => pr.Productos)
                  .HasForeignKey(p => p.ProveedorId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // Clientes
        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.ToTable("Clientes");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Documento).HasMaxLength(20).IsRequired();
            entity.HasIndex(c => c.Documento).IsUnique();
            entity.Property(c => c.NombreCompleto).HasMaxLength(150).IsRequired();
        });

        // Ventas
        modelBuilder.Entity<Venta>(entity =>
        {
            entity.ToTable("Ventas");
            entity.HasKey(v => v.Id);
            entity.Property(v => v.NumeroBoleta).HasMaxLength(50).IsRequired();
            entity.HasIndex(v => v.NumeroBoleta).IsUnique();
            entity.Property(v => v.Subtotal).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(v => v.Igv).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(v => v.Total).HasColumnType("decimal(18,2)").IsRequired();

            entity.HasOne(v => v.Cliente)
                  .WithMany(c => c.Ventas)
                  .HasForeignKey(v => v.ClienteId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(v => v.Usuario)
                  .WithMany(u => u.Ventas)
                  .HasForeignKey(v => v.UsuarioId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // DetalleVentas
        modelBuilder.Entity<DetalleVenta>(entity =>
        {
            entity.ToTable("DetalleVentas");
            entity.HasKey(d => d.Id);
            entity.Property(d => d.NombreProducto).HasMaxLength(150).IsRequired();
            entity.Property(d => d.Cantidad).HasColumnType("decimal(10,3)").IsRequired();
            entity.Property(d => d.PrecioUnitario).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(d => d.CostoUnitario).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(d => d.Subtotal).HasColumnType("decimal(18,2)").IsRequired();

            entity.HasOne(d => d.Venta)
                  .WithMany(v => v.Detalles)
                  .HasForeignKey(d => d.VentaId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.Producto)
                  .WithMany(p => p.DetallesVenta)
                  .HasForeignKey(d => d.ProductoId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // Kardex
        modelBuilder.Entity<Kardex>(entity =>
        {
            entity.ToTable("Kardex");
            entity.HasKey(k => k.Id);
            entity.Property(k => k.TipoMovimiento).HasMaxLength(20).IsRequired();
            entity.Property(k => k.Cantidad).HasColumnType("decimal(10,3)").IsRequired();
            entity.Property(k => k.Saldo).HasColumnType("decimal(10,3)").IsRequired();
            entity.Property(k => k.Motivo).HasMaxLength(200).IsRequired();

            entity.HasOne(k => k.Producto)
                  .WithMany(p => p.MovimientosKardex)
                  .HasForeignKey(k => k.ProductoId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(k => k.Usuario)
                  .WithMany(u => u.MovimientosKardex)
                  .HasForeignKey(k => k.UsuarioId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Configuracion
        modelBuilder.Entity<ConfiguracionTienda>(entity =>
        {
            entity.ToTable("Configuracion");
            entity.HasKey(cfg => cfg.Id);
            entity.Property(cfg => cfg.NombreComercio).HasMaxLength(150).IsRequired();
            entity.Property(cfg => cfg.TasaIgv).HasColumnType("decimal(5,2)").IsRequired();
        });
    }
}
