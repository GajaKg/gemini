using gemini.Interfaces;
using gemini.Models;
using Microsoft.EntityFrameworkCore;

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
                .HasIndex(e => new
                {
                    e.CurrencyId,
                    e.Date,
                })
                .IsUnique();

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
                }
            );

            base.OnModelCreating(modelBuilder);
        }
    }
}