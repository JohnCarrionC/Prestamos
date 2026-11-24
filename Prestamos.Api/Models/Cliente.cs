using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Prestamos.Api.Models;

[Table("Cliente")]
public class Cliente
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("NombreCompleto")]
    [MaxLength(100)]
    public string NombreCompleto { get; set; } = string.Empty;

    [Required]
    [Column("Correo")]
    [MaxLength(150)]
    public string Correo { get; set; } = string.Empty;

    // Se usa para decidir si el cliente puede pagar un nuevo préstamo
    [Column("IngresoMensual", TypeName = "decimal(18,2)")]
    public decimal IngresoMensual { get; set; }

    // Relación 1 a muchos: un cliente tiene muchos préstamos
    public ICollection<Prestamo> Prestamos { get; set; } = new List<Prestamo>();
}
