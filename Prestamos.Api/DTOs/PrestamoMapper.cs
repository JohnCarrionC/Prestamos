using Prestamos.Api.Models;

namespace Prestamos.Api.DTOs;

// Conversión de entidades de base de datos a DTOs
public static class PrestamoMapper
{
    public static ClienteDto ToClienteDto(Cliente cliente) => new ClienteDto
    {
        Id = cliente.Id,
        NombreCompleto = cliente.NombreCompleto,
        Correo = cliente.Correo,
        IngresoMensual = cliente.IngresoMensual
    };

    public static PrestamoResumenDto ToResumenDto(Prestamo prestamo) => new PrestamoResumenDto
    {
        Id = prestamo.Id,
        ClienteId = prestamo.ClienteId,
        NombreCliente = prestamo.Cliente.NombreCompleto,
        Monto = prestamo.Monto,
        TasaAnual = prestamo.TasaAnual,
        PlazoMeses = prestamo.PlazoMeses,
        FechaInicio = prestamo.FechaInicio,
        CuotaMensual = prestamo.CuotaMensual,
        TotalAPagar = prestamo.TotalAPagar,
        Estado = prestamo.Estado,
        CuotasPagadas = prestamo.Cuotas.Count(c => c.FechaPago != null)
    };

    public static PrestamoDto ToPrestamoDto(Prestamo prestamo) => new PrestamoDto
    {
        Id = prestamo.Id,
        ClienteId = prestamo.ClienteId,
        NombreCliente = prestamo.Cliente.NombreCompleto,
        Monto = prestamo.Monto,
        TasaAnual = prestamo.TasaAnual,
        PlazoMeses = prestamo.PlazoMeses,
        FechaInicio = prestamo.FechaInicio,
        CuotaMensual = prestamo.CuotaMensual,
        TotalAPagar = prestamo.TotalAPagar,
        Estado = prestamo.Estado,
        CuotasPagadas = prestamo.Cuotas.Count(c => c.FechaPago != null),
        Cuotas = prestamo.Cuotas
            .OrderBy(c => c.Numero)
            .Select(c => new CuotaDto
            {
                Id = c.Id,
                Numero = c.Numero,
                FechaVencimiento = c.FechaVencimiento,
                Monto = c.Monto,
                FechaPago = c.FechaPago,
                Pagada = c.FechaPago != null
            })
            .ToList()
    };
}
