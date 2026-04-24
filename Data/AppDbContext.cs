using Microsoft.EntityFrameworkCore;
using CacauShowApi.Models;

namespace CacauShowApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Produto> Produtos { get; set; }
    public DbSet<Franquia> Franquias { get; set; }
    public DbSet<LoteProducao> LotesProducao { get; set; }
    public DbSet<Pedido> Pedidos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Pedido>()
            .HasOne(p => p.Unidade)
            .WithMany(f => f.Pedidos)
            .HasForeignKey(p => p.UnidadeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Pedido>()
            .HasOne(p => p.Produto)
            .WithMany(pr => pr.Pedidos)
            .HasForeignKey(p => p.ProdutoId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<LoteProducao>()
            .HasOne(l => l.Produto)
            .WithMany(p => p.Lotes)
            .HasForeignKey(l => l.ProdutoId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Produto>()
            .Property(p => p.PrecoBase)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Pedido>()
            .Property(p => p.ValorTotal)
            .HasColumnType("decimal(18,2)");
    }
}
