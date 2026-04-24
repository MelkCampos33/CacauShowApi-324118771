using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CacauShowApi.Data;
using CacauShowApi.Models;

namespace CacauShowApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FranquiasController : ControllerBase
{
    private readonly AppDbContext _context;

    public FranquiasController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Franquia>>> GetAll()
    {
        return await _context.Franquias.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Franquia>> GetById(int id)
    {
        var franquia = await _context.Franquias.FindAsync(id);
        if (franquia == null) return NotFound();
        return franquia;
    }

    [HttpPost]
    public async Task<ActionResult<Franquia>> Create(Franquia franquia)
    {
        if (franquia.CapacidadeEstoque <= 0)
            return BadRequest("CapacidadeEstoque deve ser maior que zero.");

        _context.Franquias.Add(franquia);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = franquia.Id }, franquia);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Franquia>> Update(int id, Franquia franquia)
    {
        if (id != franquia.Id) return BadRequest();
        if (franquia.CapacidadeEstoque <= 0)
            return BadRequest("CapacidadeEstoque deve ser maior que zero.");

        _context.Entry(franquia).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return Ok(franquia);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var franquia = await _context.Franquias.FindAsync(id);
        if (franquia == null) return NotFound();
        _context.Franquias.Remove(franquia);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
