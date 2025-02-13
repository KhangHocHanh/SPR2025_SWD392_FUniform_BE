using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ClothingCustomization.Data;

public partial class ClothesCusShopContext : DbContext
{
    public ClothesCusShopContext()
    {
    }

    public ClothesCusShopContext(DbContextOptions<ClothesCusShopContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<CustomizeProduct> CustomizeProducts { get; set; }

    public virtual DbSet<DesignArea> DesignAreas { get; set; }

    public virtual DbSet<DesignElement> DesignElements { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderStage> OrderStages { get; set; }

    public virtual DbSet<OrderStageName> OrderStageNames { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=ClothesCusShop;Integrated Security=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Category__6DB38D4E0C1A4D72");

            entity.ToTable("Category");

            entity.Property(e => e.CategoryId).HasColumnName("Category_ID");
            entity.Property(e => e.CategoryName).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(50);
        });

        modelBuilder.Entity<CustomizeProduct>(entity =>
        {
            entity.HasKey(e => e.CustomizeProductId).HasName("PK__Customiz__21B784E8CACCA874");

            entity.ToTable("CustomizeProduct");

            entity.Property(e => e.CustomizeProductId).HasColumnName("CustomizeProduct_ID");
            entity.Property(e => e.Description).HasMaxLength(250);
            entity.Property(e => e.FullImage).HasMaxLength(250);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ProductId).HasColumnName("Product_ID");
            entity.Property(e => e.ShirtColor).HasMaxLength(50);
            entity.Property(e => e.UserId).HasColumnName("User_ID");

            entity.HasOne(d => d.Product).WithMany(p => p.CustomizeProducts)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CustomizeProduct_Product");

            entity.HasOne(d => d.User).WithMany(p => p.CustomizeProducts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CustomizeProduct_User");
        });

        modelBuilder.Entity<DesignArea>(entity =>
        {
            entity.HasKey(e => e.DesignAreaId).HasName("PK__DesignAr__FA8C6C4CE3B13A5D");

            entity.ToTable("DesignArea");

            entity.Property(e => e.DesignAreaId).HasColumnName("DesignArea_ID");
            entity.Property(e => e.AreaName).HasMaxLength(50);
            entity.Property(e => e.CoordinateX).HasMaxLength(20);
            entity.Property(e => e.CoordinateY).HasMaxLength(20);
            entity.Property(e => e.ProductId).HasColumnName("Product_ID");

            entity.HasOne(d => d.Product).WithMany(p => p.DesignAreas)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DesignArea_Product");
        });

        modelBuilder.Entity<DesignElement>(entity =>
        {
            entity.HasKey(e => e.DesignElementId).HasName("PK__DesignEl__4221EE4DBF03B088");

            entity.ToTable("DesignElement");

            entity.Property(e => e.DesignElementId).HasColumnName("DesignElement_ID");
            entity.Property(e => e.ColorArea).HasMaxLength(20);
            entity.Property(e => e.CustomizeProductId).HasColumnName("CustomizeProduct_ID");
            entity.Property(e => e.DesignAreaId).HasColumnName("DesignArea_ID");
            entity.Property(e => e.Image).HasMaxLength(250);
            entity.Property(e => e.Size).HasMaxLength(10);
            entity.Property(e => e.Text).HasMaxLength(250);

            entity.HasOne(d => d.CustomizeProduct).WithMany(p => p.DesignElements)
                .HasForeignKey(d => d.CustomizeProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DesignElement_CustomizeProduct");

            entity.HasOne(d => d.DesignArea).WithMany(p => p.DesignElements)
                .HasForeignKey(d => d.DesignAreaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DesignElement_DesignArea");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.FeedbackId).HasName("PK__Feedback__CD3992F85D43AD42");

            entity.ToTable("Feedback");

            entity.Property(e => e.FeedbackId).HasColumnName("Feedback_ID");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.OrderId).HasColumnName("Order_ID");
            entity.Property(e => e.Review).HasMaxLength(250);
            entity.Property(e => e.UserId).HasColumnName("User_ID");

            entity.HasOne(d => d.User).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Feedbacks_User");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__8C1160B5C309290A");

            entity.ToTable("Notification");

            entity.Property(e => e.NotificationId).HasColumnName("Notification_ID");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Message).HasMaxLength(50);
            entity.Property(e => e.Subject).HasMaxLength(50);
            entity.Property(e => e.UserId).HasColumnName("User_ID");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Notification_User");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Order__F1E4639BAEFB80EE");

            entity.ToTable("Order");

            entity.Property(e => e.OrderId).HasColumnName("Order_ID");
            entity.Property(e => e.CustomizeProductId).HasColumnName("CustomizeProduct_ID");
            entity.Property(e => e.DeliveryAddress).HasMaxLength(250);
            entity.Property(e => e.DeliveryDate).HasColumnType("datetime");
            entity.Property(e => e.Notes).HasMaxLength(250);
            entity.Property(e => e.OrderDate).HasColumnType("datetime");
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RecipientName).HasMaxLength(250);
            entity.Property(e => e.ShippingMethod).HasMaxLength(50);
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.CustomizeProduct).WithMany(p => p.Orders)
                .HasForeignKey(d => d.CustomizeProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Order_CustomizeProduct");
        });

        modelBuilder.Entity<OrderStage>(entity =>
        {
            entity.HasKey(e => e.OrderStageId).HasName("PK__OrderSta__B46B8F72B05A437B");

            entity.ToTable("OrderStage");

            entity.Property(e => e.OrderStageId).HasColumnName("OrderStage_ID");
            entity.Property(e => e.OrderId).HasColumnName("Order_ID");
            entity.Property(e => e.OrderStageNameId).HasColumnName("OrderStageName_ID");
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderStages)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderStage_Order");

            entity.HasOne(d => d.OrderStageName).WithMany(p => p.OrderStages)
                .HasForeignKey(d => d.OrderStageNameId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderStage_OrderStageName");
        });

        modelBuilder.Entity<OrderStageName>(entity =>
        {
            entity.HasKey(e => e.OrderStageNameId).HasName("PK__OrderSta__2DC4BA36C3ADDC41");

            entity.ToTable("OrderStageName");

            entity.Property(e => e.OrderStageNameId).HasColumnName("OrderStageName_ID");
            entity.Property(e => e.StatusName).HasMaxLength(50);
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payment__DA6C7FE1156D69C6");

            entity.ToTable("Payment");

            entity.Property(e => e.PaymentId).HasColumnName("Payment_ID");
            entity.Property(e => e.DepositAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DepositPaid).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.OrderId).HasColumnName("Order_ID");
            entity.Property(e => e.PaymentDate).HasColumnType("datetime");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Order).WithMany(p => p.Payments)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Payment_Order");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Product__9834FB9A1FAC92B9");

            entity.ToTable("Product");

            entity.Property(e => e.ProductId).HasColumnName("Product_ID");
            entity.Property(e => e.CategoryId).HasColumnName("Category_ID");
            entity.Property(e => e.Image).HasMaxLength(250);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ProductName).HasMaxLength(50);

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Product_Category");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Role__D80AB49B3DF69F5E");

            entity.ToTable("Role");

            entity.Property(e => e.RoleId).HasColumnName("Role_ID");
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__206D9190678239A7");

            entity.ToTable("User");

            entity.Property(e => e.UserId).HasColumnName("User_ID");
            entity.Property(e => e.Address).HasMaxLength(60);
            entity.Property(e => e.Avatar).HasMaxLength(50);
            entity.Property(e => e.DateOfBirth).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.FullName).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(50);
            entity.Property(e => e.Phone).HasMaxLength(24);
            entity.Property(e => e.RoleId).HasColumnName("Role_ID");
            entity.Property(e => e.Username).HasMaxLength(50);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_Role");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
