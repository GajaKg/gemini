using Microsoft.EntityFrameworkCore;
using Scrap.Domain.Interfaces;
using Scrap.Domain.Models;

namespace gemini.Data
{
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
                .HasOne(e => e.Currency)
                .WithMany(c => c.ExchangeRates)
                .HasForeignKey(e => e.CurrencyId)
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<ExchangeRate>()
                .HasOne(e => e.TargetCurrency)
                .WithMany()
                .HasForeignKey(e => e.TargetCurrencyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ExchangeRate>()
                .HasIndex(e => new
                {
                    e.CurrencyId,
                    e.TargetCurrencyId,
                    e.Date,
                })
                .IsUnique();

            modelBuilder.Entity<Currency>()
                .HasIndex(c => new
                {
                    c.Code,
                })
                .IsUnique();

            modelBuilder.Entity<Currency>()
                .Property(c => c.Code)
                .HasConversion<string>();

            modelBuilder.Entity<Currency>().HasData(
                new Currency
                {
                    Id = 1,
                    Code = CurrencyNames.XOF,
                    Name = CurrencyNames.XOFFullName
                },
                new Currency
                {
                    Id = 2,
                    Code = CurrencyNames.MAD,
                    Name = CurrencyNames.MADFullName
                },
                new Currency
                {
                    Id = 3,
                    Code = CurrencyNames.EUR,
                    Name = CurrencyNames.EURFullName
                },
                new Currency
                {
                    Id = 4,
                    Code = CurrencyNames.USD,
                    Name = CurrencyNames.USDFullName
                }
            );

            base.OnModelCreating(modelBuilder);
        }
    }
}