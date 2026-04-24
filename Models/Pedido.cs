namespace CacauShowApi.Models;

public class Pedido
{
    public int Id { get; set; }
    public int UnidadeId { get; set; }
    public int ProdutoId { get; set; }
    public int Quantidade { get; set; }
    public decimal ValorTotal { get; set; }

    public Franquia? Unidade { get; set; }
    public Produto? Produto { get; set; }
}
