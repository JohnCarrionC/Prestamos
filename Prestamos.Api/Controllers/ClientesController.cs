using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Prestamos.Api.Data;
using Prestamos.Api.DTOs;
using Prestamos.Api.Models;

namespace Prestamos.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientesController : ControllerBase
{
    private readonly AppDbContext _context;

    public ClientesController(AppDbContext context)
    {
        _context = context;
    }

    
    [HttpGet]
    public async Task<ActionResult<List<ClienteDto>>> ObtenerClientes()
    {
        var clientes = await _context.Clientes.ToListAsync();
        return Ok(clientes.Select(PrestamoMapper.ToClienteDto).ToList());
    }

    // GET api/clientes/1
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ClienteDto>> ObtenerCliente(int id)
    {
        var cliente = await _context.Clientes.FindAsync(id);
        if (cliente == null)
        {
            return NotFound();
        }

        return Ok(PrestamoMapper.ToClienteDto(cliente));
    }

    // POST api/clientes
    [HttpPost]
    public async Task<ActionResult<ClienteDto>> CrearCliente(ClienteDto dto)
    {
        var correoExiste = await _context.Clientes.AnyAsync(c => c.Correo == dto.Correo);
        if (correoExiste)
        {
            return Conflict("Ya existe un cliente con ese correo.");
        }

        var cliente = new Cliente
        {
            NombreCompleto = dto.NombreCompleto,
            Correo = dto.Correo,
            IngresoMensual = dto.IngresoMensual
        };

        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();

        // Tras el Save, EF ya le asignó el Id automáticamente
        return CreatedAtAction(nameof(ObtenerCliente), new { id = cliente.Id }, PrestamoMapper.ToClienteDto(cliente));
    }

    // PUT api/clientes/1
    [HttpPut("{id:int}")]
    public async Task<IActionResult> ActualizarCliente(int id, ClienteDto dto)
    {
        if (id != dto.Id)
        {
            return BadRequest("El id de la URL no coincide con el del cuerpo.");
        }

        var cliente = await _context.Clientes.FindAsync(id);
        if (cliente == null)
        {
            return NotFound();
        }

        var correoTomado = await _context.Clientes.AnyAsync(c => c.Correo == dto.Correo && c.Id != id);
        if (correoTomado)
        {
            return Conflict("Ya existe otro cliente con ese correo.");
        }

        cliente.NombreCompleto = dto.NombreCompleto;
        cliente.Correo = dto.Correo;
        cliente.IngresoMensual = dto.IngresoMensual;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE api/clientes/1
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> EliminarCliente(int id)
    {
        var cliente = await _context.Clientes.FindAsync(id);
        if (cliente == null)
        {
            return NotFound();
        }

        var tienePrestamos = await _context.Prestamos.AnyAsync(p => p.ClienteId == id);
        if (tienePrestamos)
        {
            return Conflict("No se puede eliminar un cliente que tiene préstamos registrados.");
        }

        _context.Clientes.Remove(cliente);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
