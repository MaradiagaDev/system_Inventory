using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace System_Inventory.ModelsSystemInventory;

public partial class InventorySystemContext : DbContext
{
    public InventorySystemContext()
    {
    }

    public InventorySystemContext(DbContextOptions<InventorySystemContext> options)
        : base(options)
    {
    }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=Inventory_system.mssql.somee.com;Database=Inventory_system;User Id=Rolando03_SQLLogin_1;Password=Facil123$;Encrypt=True;TrustServerCertificate = True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCAC5E1B2FB5");

            entity.ToTable("Users", "HR");

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ModificationDate).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.RolUser)
                .HasMaxLength(50)
                .HasDefaultValueSql("('Operator')");
            entity.Property(e => e.StatusUser)
                .HasMaxLength(50)
                .HasDefaultValueSql("('Active')");
            entity.Property(e => e.UserName).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
