using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MiniShipmentTracking.Database.AppDbContextModels;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TblLogin> TblLogins { get; set; }

    public virtual DbSet<TblShipment> TblShipments { get; set; }

    public virtual DbSet<TblTrackingEvent> TblTrackingEvents { get; set; }

    public virtual DbSet<TblUser> TblUsers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.;Database=ShipmentTracking;User ID=sa;Password=sasa@123;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TblLogin>(entity =>
        {
            entity.HasKey(e => e.LoginId).HasName("PK__Tbl_Logi__4DDA2818A17BD3C1");

            entity.ToTable("Tbl_Login");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.SessionExpiredAt).HasColumnType("datetime");
            entity.Property(e => e.SessionId)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TblShipment>(entity =>
        {
            entity.HasKey(e => e.ShipmentId).HasName("PK__Tbl_Ship__5CAD37ED7126EF9B");

            entity.ToTable("Tbl_Shipment");

            entity.HasIndex(e => e.TrackingNo, "UQ__Tbl_Ship__3C1E044BB216CBEA").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Destination)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Origin)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.TrackingNo)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<TblTrackingEvent>(entity =>
        {
            entity.HasKey(e => e.EventId).HasName("PK__Tracking__7944C8108D90682B");

            entity.ToTable("Tbl_TrackingEvent");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).IsUnicode(false);
            entity.Property(e => e.Location)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TblUser>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Tbl_User__1788CC4C9B84FC1D");

            entity.ToTable("Tbl_User");

            entity.HasIndex(e => e.Email, "UQ__Tbl_User__A9D10534C1C20E45").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
