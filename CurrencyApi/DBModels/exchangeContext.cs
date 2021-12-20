using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CurrencyApi.DBModels
{
    public partial class exchangeContext : DbContext
    {
        private string _connectionString;
        public exchangeContext(string connString)
        {
            _connectionString = connString;
        }

        public exchangeContext(DbContextOptions<exchangeContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Quote> Quotes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(_connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Quote>(entity =>
            {
                entity.ToTable("quotes");

                entity.HasIndex(e => new { e.Date, e.Valuteid }, "quotes_const")
                    .IsUnique();

                entity.HasIndex(e => new { e.Date, e.Name, e.Valuteid }, "quotes_date_name_valuteid_idx");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Charcode)
                    .IsRequired()
                    .HasColumnName("charcode");

                entity.Property(e => e.Date)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("date");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");

                entity.Property(e => e.Nominal).HasColumnName("nominal");

                entity.Property(e => e.Numcode).HasColumnName("numcode");

                entity.Property(e => e.Value).HasColumnName("value");

                entity.Property(e => e.Valuteid)
                    .IsRequired()
                    .HasColumnName("valuteid");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
