using Microsoft.EntityFrameworkCore;
using Readex.Models;

namespace Readex.Infraestrutura;

public sealed class ReadexDbContext : DbContext {
    public ReadexDbContext(DbContextOptions<ReadexDbContext> options)
        : base(options) {
    }

    public DbSet<Obra> Obras => Set<Obra>();
    public DbSet<Leitor> Leitores => Set<Leitor>();
    public DbSet<Lista> Listas => Set<Lista>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Obra>().HasIndex(obra => obra.Slug).IsUnique();

        modelBuilder.Entity<Leitor>(leitor =>
        {
            leitor.Property(item => item.Id).ValueGeneratedNever();
            leitor.Property(item => item.NomeUsuario).HasMaxLength(30);
            leitor.Property(item => item.Email).HasMaxLength(254);
            leitor.Property(item => item.NomeUsuarioNormalizado).HasMaxLength(30);
            leitor.Property(item => item.EmailNormalizado).HasMaxLength(254);
            leitor.Property(item => item.SenhaHash).HasMaxLength(512);
            leitor.Property(item => item.NomeExibicao).HasMaxLength(60);
            leitor.Property(item => item.FotoUrl).HasMaxLength(500);
            leitor.HasIndex(item => item.NomeUsuarioNormalizado).IsUnique();
            leitor.HasIndex(item => item.EmailNormalizado).IsUnique();

            // Ratings and profile activity are still handled by the domain model.
            leitor.Ignore(item => item.Avaliacoes);
            leitor.Ignore(item => item.Curtidas);
            leitor.Ignore(item => item.Favoritos);

            leitor.HasMany(item => item.Listas)
                .WithOne()
                .HasForeignKey(lista => lista.LeitorId)
                .OnDelete(DeleteBehavior.Cascade);
            leitor.Navigation(item => item.Listas)
                .HasField("_listas")
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        });

        modelBuilder.Entity<Lista>(lista =>
        {
            lista.Property(item => item.Id).ValueGeneratedNever();
            lista.Property(item => item.Nome).HasMaxLength(80);
            lista.Ignore(item => item.Itens);
            lista.HasIndex(item => new { item.LeitorId, item.Tipo })
                .IsUnique()
                .HasFilter("\"Tipo\" = 0");
        });
    }
}
