using System.ComponentModel.DataAnnotations;

namespace Prestamos.Api.DTOs;

// Se usa para entrada y salida de clientes
public class ClienteDto
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string NombreCompleto { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(150)]
    public string Correo { get; set; } = string.Empty;

    [Range(1, 1_000_000)]
    public decimal IngresoMensual { get; set; }
}
