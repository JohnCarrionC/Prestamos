using Microsoft.EntityFrameworkCore;
using Prestamos.Api.Models;

namespace Prestamos.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Prestamo> Prestamos { get; set; }
    public DbSet<Cuota> Cuotas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Lo que no se puede expresar con atributos va aquí (modo fluent):

        // El estado se guarda como texto ("Activo"/"Pagado") en vez de 0/1
        modelBuilder.Entity<Prestamo>()
            .Property(p => p.Estado)
            .HasConversion<string>();

        // No puede haber dos clientes con el mismo correo
        modelBuilder.Entity<Cliente>()
            .HasIndex(c => c.Correo)
            .IsUnique();

        // No puede haber dos cuotas con el mismo número dentro de un préstamo
        modelBuilder.Entity<Cuota>()
            .HasIndex(c => new { c.PrestamoId, c.Numero })
            .IsUnique();
    }
}
