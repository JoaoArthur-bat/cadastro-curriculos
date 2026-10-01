using Curriculos.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Curriculos.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Candidato> Candidatos => Set<Candidato>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Server=localhost,1433;Database=Curriculos;User Id=sa;Password=Curriculos@123;TrustServerCertificate=True");
        }
    }

    protected override void OnModelCreating(ModelBuilder mb)
    {
        base.OnModelCreating(mb);

        mb.Entity<Candidato>(e =>
        {
            e.Property(c => c.NomeCompleto).HasMaxLength(200).IsRequired();
            e.Property(c => c.Email).HasMaxLength(200).IsRequired();
            e.Property(c => c.Telefone).HasMaxLength(30);
            e.Property(c => c.AreaInteresse).HasMaxLength(150);
            e.Property(c => c.ResumoProfissional).HasMaxLength(2000);
        });
    }
}