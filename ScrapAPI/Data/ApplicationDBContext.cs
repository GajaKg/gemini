

using Microsoft.EntityFrameworkCore;
using Scrap.Domain.Entities;

namespace ScrapAPI.Data;

public class ApplicationDBContext : DbContext
{
    public ApplicationDBContext(
        DbContextOptions<ApplicationDBContext> options
    ) : base(options)
    {

    }

    public DbSet<ExchangeRate> ExchangeRates { get; set; }
    public DbSet<Currency> Currencies { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ModelExchangeRate(modelBuilder);

        ModelCurrency(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }

    private static void ModelExchangeRate(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ExchangeRate>()
              .HasOne(er => er.Currency)
              .WithMany(c => c.ExchangeRates)
              .HasForeignKey(er => er.CurrencyId)
              .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ExchangeRate>()
            .HasOne(er => er.TargetCurrency)
            .WithMany(c => c.TargetExchangeRates)
            .HasForeignKey(er => er.TargetCurrencyId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ExchangeRate>()
            .HasIndex(e => new
            {
                e.CurrencyId,
                e.TargetCurrencyId,
                e.Date
            })
            .IsUnique();
    }

    private static void ModelCurrency(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Currency>()
            .Property(c => c.Code)
            .HasConversion<string>();

        modelBuilder.Entity<Currency>()
            .HasIndex(c => new
            {
                c.Code,
            })
            .IsUnique();
    }
}