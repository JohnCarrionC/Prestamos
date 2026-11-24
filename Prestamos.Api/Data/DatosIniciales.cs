using Prestamos.Api.Models;

namespace Prestamos.Api.Data;

// Rellena datos de prueba al arrancar la aplicación (solo si no hay clientes)
public static class DatosIniciales
{
    public static void Inicializar(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if (context.Clientes.Any())
        {
            return;
        }

        var clientes = new List<Cliente>
        {
            new Cliente { NombreCompleto = "Alice Torres",  Correo = "alice@example.com", IngresoMensual = 3000m },
            new Cliente { NombreCompleto = "Bob Ramírez",   Correo = "bob@example.com",   IngresoMensual = 1200m }
        };

        context.Clientes.AddRange(clientes);
        context.SaveChanges();
    }
}
