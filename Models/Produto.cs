namespace CacauShowApi.Models;

public class Produto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public decimal PrecoBase { get; set; }

    public ICollection<LoteProducao> Lotes { get; set; } = new List<LoteProducao>();
    public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
}
