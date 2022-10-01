using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DiscordBot.Models
{
    public partial class DiscordBotContext : DbContext
    {
        public DiscordBotContext()
        {
        }

        public DiscordBotContext(DbContextOptions<DiscordBotContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ImageRepository> ImageRepositories { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ImageRepository>(entity =>
            {
                entity.ToTable("ImageRepository");

                entity.Property(e => e.Keyword).HasMaxLength(50);

                entity.Property(e => e.Url).HasMaxLength(500);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
