namespace CacauShowApi.Models;

public class LoteProducao
{
    public int Id { get; set; }
    public string CodigoLote { get; set; } = string.Empty;
    public DateTime DataFabricacao { get; set; }
    public int ProdutoId { get; set; }
    public string Status { get; set; } = string.Empty;

    public Produto? Produto { get; set; }
}
