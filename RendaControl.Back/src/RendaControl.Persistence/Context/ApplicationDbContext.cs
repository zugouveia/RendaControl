using Microsoft.EntityFrameworkCore;
using RendaControl.Domain.Entities;
using RendaControl.Domain.Enums;

namespace RendaControl.Persistence.Context;

public sealed class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Gasto> Gastos => Set<Gasto>();
    public DbSet<Cliente> Clientes => Set<Cliente>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Gasto>(entity =>
        {
            entity.ToTable("gastos");
            entity.HasKey(gasto => gasto.Id);

            entity.Property(gasto => gasto.ClienteId)
                .IsRequired();

            entity.Property(gasto => gasto.Descricao)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(gasto => gasto.Valor)
                .HasPrecision(14, 2)
                .IsRequired();

            entity.Property(gasto => gasto.DataVencimento)
                .IsRequired();

            entity.HasIndex(gasto => gasto.ClienteId);

            entity.HasOne<Cliente>()
                .WithMany()
                .HasForeignKey(gasto => gasto.ClienteId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.ToTable("clientes");
            entity.HasKey(cliente => cliente.Id);

            entity.Property(cliente => cliente.Nome)
                .HasMaxLength(160)
                .IsRequired();

            entity.Property(cliente => cliente.Telefone)
                .HasMaxLength(30)
                .IsRequired();

            entity.Property(cliente => cliente.Email)
                .HasMaxLength(160)
                .IsRequired();

            entity.Property(cliente => cliente.Endereco)
                .HasMaxLength(260)
                .IsRequired();

            entity.Property(cliente => cliente.QuantidadeServicos)
                .HasDefaultValue(0)
                .IsRequired();

            entity.Property(cliente => cliente.Situacao)
                .HasConversion(
                    situacao => situacao.ToString(),
                    value => Enum.Parse<SituacaoCliente>(value))
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(cliente => cliente.DataCriacao)
                .IsRequired();
        });
    }
}
