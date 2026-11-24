using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Prestamos.Api.Data;
using Prestamos.Api.DTOs;
using Prestamos.Api.Models;
using Prestamos.Api.Services;

namespace Prestamos.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PrestamosController : ControllerBase
{
    private readonly AppDbContext _context;

    public PrestamosController(AppDbContext context)
    {
        _context = context;
    }

    // GET api/prestamos/simular?monto=5000&meses=12
    // Calcula la cuota SIN guardar nada en base de datos
    [HttpGet("simular")]
    public ActionResult<SimulacionPrestamoDto> Simular([FromQuery] decimal monto, [FromQuery] int meses)
    {
        var error = ReglasPrestamo.ValidarMontoYPlazo(monto, meses);
        if (error != null)
        {
            return BadRequest(error);
        }

        var cuota = CalculadoraPrestamo.CuotaMensual(monto, ReglasPrestamo.TasaAnual, meses);
        var total = cuota * meses;

        return Ok(new SimulacionPrestamoDto
        {
            Monto = monto,
            PlazoMeses = meses,
            TasaAnual = ReglasPrestamo.TasaAnual,
            CuotaMensual = cuota,
            TotalAPagar = total,
            TotalIntereses = total - monto
        });
    }

    // GET api/prestamos
    [HttpGet]
    public async Task<ActionResult<List<PrestamoResumenDto>>> ObtenerPrestamos()
    {
        var prestamos = await _context.Prestamos
            .Include(p => p.Cliente)
            .Include(p => p.Cuotas)
            .ToListAsync();

        return Ok(prestamos.Select(PrestamoMapper.ToResumenDto).ToList());
    }

    // GET api/prestamos/1  (con el cronograma de cuotas)
    [HttpGet("{id:int}")]
    public async Task<ActionResult<PrestamoDto>> ObtenerPrestamo(int id)
    {
        var prestamo = await _context.Prestamos
            .Include(p => p.Cliente)
            .Include(p => p.Cuotas)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (prestamo == null)
        {
            return NotFound();
        }

        return Ok(PrestamoMapper.ToPrestamoDto(prestamo));
    }

    // GET api/prestamos/cliente/1
    [HttpGet("cliente/{clienteId:int}")]
    public async Task<ActionResult<List<PrestamoResumenDto>>> ObtenerPrestamosPorCliente(int clienteId)
    {
        var clienteExiste = await _context.Clientes.AnyAsync(c => c.Id == clienteId);
        if (!clienteExiste)
        {
            return NotFound($"No existe el cliente {clienteId}.");
        }

        var prestamos = await _context.Prestamos
            .Include(p => p.Cliente)
            .Include(p => p.Cuotas)
            .Where(p => p.ClienteId == clienteId)
            .ToListAsync();

        return Ok(prestamos.Select(PrestamoMapper.ToResumenDto).ToList());
    }

    // POST api/prestamos
    // Solicita un préstamo: valida las reglas, calcula la cuota y genera el cronograma
    [HttpPost]
    public async Task<ActionResult<PrestamoDto>> SolicitarPrestamo(SolicitudPrestamoDto dto)
    {
        // 1. Validar monto y plazo
        var error = ReglasPrestamo.ValidarMontoYPlazo(dto.Monto, dto.PlazoMeses);
        if (error != null)
        {
            return BadRequest(error);
        }

        // 2. El cliente debe existir
        var cliente = await _context.Clientes
            .Include(c => c.Prestamos)
            .FirstOrDefaultAsync(c => c.Id == dto.ClienteId);

        if (cliente == null)
        {
            return NotFound($"No existe el cliente {dto.ClienteId}.");
        }

        // 3. Máximo de préstamos activos
        var prestamosActivos = cliente.Prestamos
            .Where(p => p.Estado == EstadoPrestamo.Activo)
            .ToList();

        if (prestamosActivos.Count >= ReglasPrestamo.MaxPrestamosActivos)
        {
            return Conflict($"El cliente ya tiene {ReglasPrestamo.MaxPrestamosActivos} préstamos activos.");
        }

        // 4. Capacidad de pago: cuotas actuales + la nueva <= 40 % del ingreso
        var cuotaMensual = CalculadoraPrestamo.CuotaMensual(dto.Monto, ReglasPrestamo.TasaAnual, dto.PlazoMeses);
        var comprometido = prestamosActivos.Sum(p => p.CuotaMensual) + cuotaMensual;
        var maximoPermitido = cliente.IngresoMensual * ReglasPrestamo.MaxEndeudamiento;

        if (comprometido > maximoPermitido)
        {
            return UnprocessableEntity(
                $"Las cuotas mensuales ({comprometido}) superarían el {ReglasPrestamo.MaxEndeudamiento:P0} " +
                $"del ingreso del cliente (máximo permitido: {maximoPermitido}).");
        }

        // 5. Crear el préstamo con su cronograma de cuotas
        var fechaInicio = DateTime.UtcNow.Date;

        var prestamo = new Prestamo
        {
            ClienteId = cliente.Id,
            Monto = dto.Monto,
            TasaAnual = ReglasPrestamo.TasaAnual,
            PlazoMeses = dto.PlazoMeses,
            FechaInicio = fechaInicio,
            CuotaMensual = cuotaMensual,
            TotalAPagar = cuotaMensual * dto.PlazoMeses,
            Estado = EstadoPrestamo.Activo
        };

        for (var i = 1; i <= dto.PlazoMeses; i++)
        {
            prestamo.Cuotas.Add(new Cuota
            {
                Numero = i,
                FechaVencimiento = fechaInicio.AddMonths(i),
                Monto = cuotaMensual
            });
        }

        _context.Prestamos.Add(prestamo);
        await _context.SaveChangesAsync();

        prestamo.Cliente = cliente; // para que el mapper pueda leer el nombre
        return CreatedAtAction(nameof(ObtenerPrestamo), new { id = prestamo.Id }, PrestamoMapper.ToPrestamoDto(prestamo));
    }

    // POST api/prestamos/1/pagar
    // Paga la siguiente cuota pendiente del préstamo
    [HttpPost("{id:int}/pagar")]
    public async Task<IActionResult> PagarSiguienteCuota(int id)
    {
        var prestamo = await _context.Prestamos
            .Include(p => p.Cuotas)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (prestamo == null)
        {
            return NotFound();
        }

        if (prestamo.Estado == EstadoPrestamo.Pagado)
        {
            return Conflict("Este préstamo ya está pagado completamente.");
        }

        var siguiente = prestamo.Cuotas
            .Where(c => c.FechaPago == null)
            .OrderBy(c => c.Numero)
            .First();

        siguiente.FechaPago = DateTime.UtcNow;

        var pendientes = prestamo.Cuotas.Count(c => c.FechaPago == null);
        if (pendientes == 0)
        {
            prestamo.Estado = EstadoPrestamo.Pagado;
        }

        await _context.SaveChangesAsync();

        return Ok(new
        {
            CuotaPagada = siguiente.Numero,
            MontoPagado = siguiente.Monto,
            CuotasPendientes = pendientes,
            EstadoPrestamo = prestamo.Estado.ToString()
        });
    }
}
