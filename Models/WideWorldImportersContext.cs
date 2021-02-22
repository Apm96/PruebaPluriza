using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ProjectSales.Models
{
    public partial class WideWorldImportersContext : DbContext
    {

        public WideWorldImportersContext(DbContextOptions<WideWorldImportersContext> options)
            : base(options)
        {
        }
        public virtual DbSet<SaleDetail> SaleDetail { get; set; }
        public virtual DbSet<Sales> Sales { get; set; }
        public virtual DbSet<State> State { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserType> UserType { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SaleDetail>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Amount)
                    .HasColumnName("amount")
                    .HasColumnType("decimal(18, 2)");

                entity.Property(e => e.IdSales).HasColumnName("idSales");

                entity.Property(e => e.Item)
                    .HasColumnName("item")
                    .IsUnicode(false);

                entity.Property(e => e.Price)
                    .HasColumnName("price")
                    .HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.IdSalesNavigation)
                    .WithMany(p => p.SaleDetail)
                    .HasForeignKey(d => d.IdSales)
                    .HasConstraintName("FK_SaleDetail_Sales");
            });

            modelBuilder.Entity<Sales>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AuditDate)
                    .HasColumnName("auditDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Date)
                    .HasColumnName("date")
                    .HasColumnType("datetime");

                entity.Property(e => e.IdEstate).HasColumnName("idEstate");

                entity.Property(e => e.IdUser).HasColumnName("idUser");

                entity.Property(e => e.Observation)
                    .HasColumnName("observation")
                    .IsUnicode(false);

                entity.Property(e => e.Settled)
                    .HasColumnName("settled")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdEstateNavigation)
                    .WithMany(p => p.Sales)
                    .HasForeignKey(d => d.IdEstate)
                    .HasConstraintName("FK_Venta_Estado");

                entity.HasOne(d => d.IdUserNavigation)
                    .WithMany(p => p.Sales)
                    .HasForeignKey(d => d.IdUser)
                    .HasConstraintName("FK_Venta_Usuarios");
            });

            modelBuilder.Entity<State>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.IdEstate).HasColumnName("idEstate");

                entity.Property(e => e.IdUserType).HasColumnName("idUserType");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .IsUnicode(false);

                entity.Property(e => e.Password).HasColumnName("password");

                entity.Property(e => e.Userr)
                    .HasColumnName("userr")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdEstateNavigation)
                    .WithMany(p => p.User)
                    .HasForeignKey(d => d.IdEstate)
                    .HasConstraintName("FK_Usuarios_Estado");

                entity.HasOne(d => d.IdUserTypeNavigation)
                    .WithMany(p => p.User)
                    .HasForeignKey(d => d.IdUserType)
                    .HasConstraintName("FK_Usuarios_TipoUsuario");
            });

            modelBuilder.Entity<UserType>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(200)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
