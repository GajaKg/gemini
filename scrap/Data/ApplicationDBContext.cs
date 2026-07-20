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
                    e.Date
                })
                .IsUnique();

            modelBuilder.Entity<Currency>().HasData(
                new Currency
                {
                    Id = 1,
                    Code = "XOF",
                    Name = "West African CFA franc"
                },
                new Currency
                {
                    Id = 2,
                    Code = "MAD",
                    Name = "Moroccan Dirham"
                }
            );

            base.OnModelCreating(modelBuilder);
        }
    }
}