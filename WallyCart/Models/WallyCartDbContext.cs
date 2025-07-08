using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WallyCart.Models;

public partial class WallyCartDbContext : DbContext
{
    public WallyCartDbContext()
    {
    }

    public WallyCartDbContext(DbContextOptions<WallyCartDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<GroupUser> GroupUsers { get; set; }

    public virtual DbSet<List> Lists { get; set; }

    public virtual DbSet<ListItem> ListItems { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("resources/en.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            optionsBuilder.UseNpgsql(config.GetConnectionString("Default"));
        }
    }
     
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("groups_pkey");

            entity.ToTable("groups");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.GroupName).HasColumnName("group_name");
        });

        modelBuilder.Entity<GroupUser>(entity =>
        {
            entity.HasKey(e => new { e.GroupId, e.UserId }).HasName("pk_group_users");

            entity.ToTable("group_users");

            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.IsAdmin)
                .HasDefaultValue(false)
                .HasColumnName("is_admin");
            entity.Property(e => e.CreatedAt)
               .HasDefaultValueSql("now()")
               .HasColumnName("created_at");

            entity.HasOne(d => d.Group).WithMany(p => p.GroupUsers)
                .HasForeignKey(d => d.GroupId)
                .HasConstraintName("fk_group_id");

            entity.HasOne(d => d.User).WithMany(p => p.GroupUsers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_user_id");
        });

        modelBuilder.Entity<List>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("lists_pkey");

            entity.ToTable("lists");

            entity.HasIndex(e => e.GroupId, "group_id").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.GroupId).HasColumnName("group_id");

            entity.HasOne(d => d.Group).WithOne(p => p.List)
                .HasForeignKey<List>(d => d.GroupId)
                .HasConstraintName("fk_group_id");
        });

        modelBuilder.Entity<ListItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("list_items_pkey");

            entity.ToTable("list_items");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.AddedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("added_at");
            entity.Property(e => e.AddedBy).HasColumnName("added_by");
            entity.Property(e => e.ListId).HasColumnName("list_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Quantity)
                .HasDefaultValue(1)
                .HasColumnName("quantity");

            entity.HasOne(d => d.AddedByNavigation).WithMany(p => p.ListItems)
                .HasForeignKey(d => d.AddedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_list_added_by");

            entity.HasOne(d => d.List).WithMany(p => p.ListItems)
                .HasForeignKey(d => d.ListId)
                .HasConstraintName("fk_list_items_list");

            entity.HasOne(d => d.Product).WithMany(p => p.ListItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_list_product_id");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("products_pkey");

            entity.ToTable("products");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Source).HasColumnName("source");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Users_pkey");

            entity.ToTable("users");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.PhoneNumber).HasColumnName("phone_number");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
