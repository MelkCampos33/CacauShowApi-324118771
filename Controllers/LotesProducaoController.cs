using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CacauShowApi.Data;
using CacauShowApi.Models;

namespace CacauShowApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LotesProducaoController : ControllerBase
{
    private readonly AppDbContext _context;

    public LotesProducaoController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LoteProducao>>> GetAll()
    {
        return await _context.LotesProducao.Include(l => l.Produto).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<LoteProducao>> GetById(int id)
    {
        var lote = await _context.LotesProducao.Include(l => l.Produto).FirstOrDefaultAsync(l => l.Id == id);
        if (lote == null) return NotFound();
        return lote;
    }

    [HttpPost]
    public async Task<ActionResult<LoteProducao>> Create(LoteProducao lote)
    {
        var produto = await _context.Produtos.FindAsync(lote.ProdutoId);
        if (produto == null)
            return NotFound("Produto não encontrado.");

        if (lote.DataFabricacao > DateTime.Now)
            return Conflict("Lote inválido: Data de fabricação não pode ser maior que a data atual.");

        var statusValidos = new[] { "Em Produção", "Qualidade Aprovada", "Distribuído", "Descartado" };
        if (!statusValidos.Contains(lote.Status))
            return BadRequest("Status inválido. Use: Em Produção, Qualidade Aprovada, Distribuído ou Descartado.");

        _context.LotesProducao.Add(lote);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = lote.Id }, lote);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<LoteProducao>> Update(int id, LoteProducao lote)
    {
        if (id != lote.Id) return BadRequest();

        var produto = await _context.Produtos.FindAsync(lote.ProdutoId);
        if (produto == null)
            return NotFound("Produto não encontrado.");

        if (lote.DataFabricacao > DateTime.Now)
            return Conflict("Lote inválido: Data de fabricação não pode ser maior que a data atual.");

        var statusValidos = new[] { "Em Produção", "Qualidade Aprovada", "Distribuído", "Descartado" };
        if (!statusValidos.Contains(lote.Status))
            return BadRequest("Status inválido. Use: Em Produção, Qualidade Aprovada, Distribuído ou Descartado.");

        _context.Entry(lote).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return Ok(lote);
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] string novoStatus)
    {
        var lote = await _context.LotesProducao.FindAsync(id);
        if (lote == null) return NotFound();

        var statusValidos = new[] { "Em Produção", "Qualidade Aprovada", "Distribuído", "Descartado" };
        if (!statusValidos.Contains(novoStatus))
            return BadRequest("Status inválido. Use: Em Produção, Qualidade Aprovada, Distribuído ou Descartado.");

        if (lote.Status == "Descartado" && (novoStatus == "Qualidade Aprovada" || novoStatus == "Distribuído"))
            return BadRequest("Lote descartado não pode ser aprovado ou distribuído.");

        lote.Status = novoStatus;
        await _context.SaveChangesAsync();
        return Ok(lote);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var lote = await _context.LotesProducao.FindAsync(id);
        if (lote == null) return NotFound();
        _context.LotesProducao.Remove(lote);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
