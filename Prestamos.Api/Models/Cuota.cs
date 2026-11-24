using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Prestamos.Api.Models;

// Una cuota del cronograma de un préstamo
[Table("Cuota")]
public class Cuota
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("PrestamoId")]
    public int PrestamoId { get; set; }

    [ForeignKey(nameof(PrestamoId))]
    public Prestamo Prestamo { get; set; } = null!;

    // Número de cuota: 1, 2, 3...
    [Column("Numero")]
    public int Numero { get; set; }

    [Column("FechaVencimiento")]
    public DateTime FechaVencimiento { get; set; }

    [Column("Monto", TypeName = "decimal(18,2)")]
    public decimal Monto { get; set; }

    // null = todavía no se ha pagado
    [Column("FechaPago")]
    public DateTime? FechaPago { get; set; }
}
