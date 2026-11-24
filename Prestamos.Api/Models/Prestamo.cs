using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Prestamos.Api.Models;

[Table("Prestamo")]
public class Prestamo
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("ClienteId")]
    public int ClienteId { get; set; }

    [ForeignKey(nameof(ClienteId))]
    public Cliente Cliente { get; set; } = null!;

    [Column("Monto", TypeName = "decimal(18,2)")]
    public decimal Monto { get; set; }

    // 0.18 = 18 % anual. Se guarda en el préstamo porque la tasa puede cambiar
    // en el futuro y hay que conservar la que se aplicó en su momento.
    [Column("TasaAnual", TypeName = "decimal(5,4)")]
    public decimal TasaAnual { get; set; }

    [Column("PlazoMeses")]
    public int PlazoMeses { get; set; }

    [Column("FechaInicio")]
    public DateTime FechaInicio { get; set; }

    [Column("CuotaMensual", TypeName = "decimal(18,2)")]
    public decimal CuotaMensual { get; set; }

    [Column("TotalAPagar", TypeName = "decimal(18,2)")]
    public decimal TotalAPagar { get; set; }

    // Se guarda como texto ("Activo" / "Pagado"); la conversión se define en AppDbContext
    [Column("Estado", TypeName = "varchar(20)")]
    public EstadoPrestamo Estado { get; set; } = EstadoPrestamo.Activo;

    // Cronograma de cuotas
    public ICollection<Cuota> Cuotas { get; set; } = new List<Cuota>();
}
