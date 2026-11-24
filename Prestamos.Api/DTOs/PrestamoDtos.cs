using System.ComponentModel.DataAnnotations;
using Prestamos.Api.Models;

namespace Prestamos.Api.DTOs;

// Lo que envía el cliente para solicitar un préstamo.
// No incluye tasa, cuota ni total: eso lo calcula el servidor.
public class SolicitudPrestamoDto
{
    [Required]
    public int ClienteId { get; set; }

    [Required]
    public decimal Monto { get; set; }

    [Required]
    public int PlazoMeses { get; set; }
}

public class SimulacionPrestamoDto
{
    public decimal Monto { get; set; }
    public int PlazoMeses { get; set; }
    public decimal TasaAnual { get; set; }
    public decimal CuotaMensual { get; set; }
    public decimal TotalAPagar { get; set; }
    public decimal TotalIntereses { get; set; }
}

public class CuotaDto
{
    public int Id { get; set; }
    public int Numero { get; set; }
    public DateTime FechaVencimiento { get; set; }
    public decimal Monto { get; set; }
    public DateTime? FechaPago { get; set; }
    public bool Pagada { get; set; }
}

// Versión resumida (para listados)
public class PrestamoResumenDto
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public string NombreCliente { get; set; } = string.Empty;
    public decimal Monto { get; set; }
    public decimal TasaAnual { get; set; }
    public int PlazoMeses { get; set; }
    public DateTime FechaInicio { get; set; }
    public decimal CuotaMensual { get; set; }
    public decimal TotalAPagar { get; set; }
    public EstadoPrestamo Estado { get; set; }
    public int CuotasPagadas { get; set; }
}

// Versión completa (con el cronograma de cuotas)
public class PrestamoDto : PrestamoResumenDto
{
    public List<CuotaDto> Cuotas { get; set; } = new();
}
