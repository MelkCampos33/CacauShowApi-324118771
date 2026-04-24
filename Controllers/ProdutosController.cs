using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CacauShowApi.Data;
using CacauShowApi.Models;

namespace CacauShowApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProdutosController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProdutosController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Produto>>> GetAll()
    {
        return await _context.Produtos.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Produto>> GetById(int id)
    {
        var produto = await _context.Produtos.FindAsync(id);
        if (produto == null) return NotFound();
        return produto;
    }

    [HttpPost]
    public async Task<ActionResult<Produto>> Create(Produto produto)
    {
        var tiposValidos = new[] { "Gourmet", "Linha Regular", "Sazonal" };
        if (!tiposValidos.Contains(produto.Tipo))
            return BadRequest("Tipo inválido. Use: Gourmet, Linha Regular ou Sazonal.");

        _context.Produtos.Add(produto);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = produto.Id }, produto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Produto>> Update(int id, Produto produto)
    {
        if (id != produto.Id) return BadRequest();

        var tiposValidos = new[] { "Gourmet", "Linha Regular", "Sazonal" };
        if (!tiposValidos.Contains(produto.Tipo))
            return BadRequest("Tipo inválido. Use: Gourmet, Linha Regular ou Sazonal.");

        _context.Entry(produto).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return Ok(produto);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var produto = await _context.Produtos.FindAsync(id);
        if (produto == null) return NotFound();
        _context.Produtos.Remove(produto);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
