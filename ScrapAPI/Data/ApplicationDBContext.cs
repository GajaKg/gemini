

using Microsoft.EntityFrameworkCore;
using Scrap.Domain.Models;

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

        modelBuilder.Entity<Currency>()
            .HasIndex(c => new
            {
                c.Code,
            })
            .IsUnique();


        base.OnModelCreating(modelBuilder);
    }

}