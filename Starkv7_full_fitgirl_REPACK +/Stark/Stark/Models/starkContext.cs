using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Stark.Models
{
    public partial class starkContext : DbContext
    {
        public starkContext()
        {
        }

        public starkContext(DbContextOptions<starkContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Badge> Badge { get; set; }
        public virtual DbSet<Cars> Cars { get; set; }
        public virtual DbSet<Review> Review { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=89.38.209.13;Database=stark;User ID=starkuser;Password=aiqkjscnmpwvluoyzbegrdxtfh;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:DefaultSchema", "starkuser");

            modelBuilder.Entity<Badge>(entity =>
            {
                entity.ToTable("Badge", "dbo");

                entity.Property(e => e.Description).IsUnicode(false);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Type).HasColumnName("type");
            });

            modelBuilder.Entity<Cars>(entity =>
            {
                entity.HasKey(e => e.LicenceId);

                entity.ToTable("Cars", "dbo");

                entity.Property(e => e.Plate)
                    .IsRequired()
                    .HasMaxLength(7)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Review>(entity =>
            {
                entity.ToTable("Review", "dbo");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("date")
                    .HasDefaultValueSql("getdate()");

                entity.Property(e => e.UserIp)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Badge)
                    .WithMany(p => p.Review)
                    .HasForeignKey(d => d.BadgeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Review_Badge");

                entity.HasOne(d => d.Licence)
                    .WithMany(p => p.Review)
                    .HasForeignKey(d => d.LicenceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Review_Cars");
            });
        }
    }
}
