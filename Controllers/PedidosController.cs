using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CacauShowApi.Data;
using CacauShowApi.Models;

namespace CacauShowApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PedidosController : ControllerBase
{
    private readonly AppDbContext _context;

    public PedidosController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Pedido>>> GetAll()
    {
        return await _context.Pedidos
            .Include(p => p.Produto)
            .Include(p => p.Unidade)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Pedido>> GetById(int id)
    {
        var pedido = await _context.Pedidos
            .Include(p => p.Produto)
            .Include(p => p.Unidade)
            .FirstOrDefaultAsync(p => p.Id == id);
        if (pedido == null) return NotFound();
        return pedido;
    }

    [HttpPost]
    public async Task<ActionResult<Pedido>> Create(Pedido pedido)
    {
        var franquia = await _context.Franquias.FindAsync(pedido.UnidadeId);
        if (franquia == null)
            return NotFound("Franquia não encontrada.");

        var produto = await _context.Produtos.FindAsync(pedido.ProdutoId);
        if (produto == null)
            return NotFound("Produto não encontrado.");

        var totalAtual = await _context.Pedidos
            .Where(p => p.UnidadeId == pedido.UnidadeId)
            .SumAsync(p => p.Quantidade);

        if (totalAtual + pedido.Quantidade > franquia.CapacidadeEstoque)
            return BadRequest("Capacidade logística da loja excedida. Não é possível receber mais produtos.");

        pedido.ValorTotal = produto.PrecoBase * pedido.Quantidade;

        if (produto.Tipo == "Sazonal")
        {
            pedido.ValorTotal += 15.00m;
            Console.WriteLine("Produto sazonal detectado: Adicionando embalagem de presente premium!");
        }

        _context.Pedidos.Add(pedido);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = pedido.Id }, pedido);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Pedido>> Update(int id, Pedido pedido)
    {
        if (id != pedido.Id) return BadRequest();

        var franquia = await _context.Franquias.FindAsync(pedido.UnidadeId);
        if (franquia == null)
            return NotFound("Franquia não encontrada.");

        var produto = await _context.Produtos.FindAsync(pedido.ProdutoId);
        if (produto == null)
            return NotFound("Produto não encontrado.");

        var totalAtual = await _context.Pedidos
            .Where(p => p.UnidadeId == pedido.UnidadeId && p.Id != id)
            .SumAsync(p => p.Quantidade);

        if (totalAtual + pedido.Quantidade > franquia.CapacidadeEstoque)
            return BadRequest("Capacidade logística da loja excedida. Não é possível receber mais produtos.");

        pedido.ValorTotal = produto.PrecoBase * pedido.Quantidade;

        if (produto.Tipo == "Sazonal")
        {
            pedido.ValorTotal += 15.00m;
            Console.WriteLine("Produto sazonal detectado: Adicionando embalagem de presente premium!");
        }

        _context.Entry(pedido).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return Ok(pedido);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var pedido = await _context.Pedidos.FindAsync(id);
        if (pedido == null) return NotFound();
        _context.Pedidos.Remove(pedido);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
