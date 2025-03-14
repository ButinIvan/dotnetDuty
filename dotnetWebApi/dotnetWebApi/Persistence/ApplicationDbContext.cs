﻿using dotnetWebApi.Entities;
using dotnetWebApi.Persistence.Сonfigurations;
using Microsoft.EntityFrameworkCore;

namespace dotnetWebApi.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
        : base(options) { }

    public DbSet<User> Users { get; set; }

    public DbSet<Document> Documents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //modelBuilder.ApplyConfiguration(new UserConfiguration());
        //modelBuilder.ApplyConfiguration(new DocumentConfiguration());
        base.OnModelCreating(modelBuilder);
        // Конфигурация для User
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Id).HasDefaultValueSql("gen_random_uuid()");

            // Обязательные поля
            entity.Property(u => u.UserName)
                .IsRequired()
                .HasMaxLength(256);

            entity.Property(u => u.FirstName).IsRequired();
            entity.Property(u => u.Role).IsRequired();
            entity.Property(u => u.PasswordHash).IsRequired();

            // Связь: User -> Documents (один ко многим)
            entity.HasMany(u => u.Documents)
                .WithOne(d => d.Owner)
                .HasForeignKey(d => d.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Связь: User -> ReviewDocuments (многие ко многим)
            entity.HasMany(u => u.ReviewDocuments)
                .WithMany(d => d.Reviewers);
        });

        // Конфигурация для Document
        modelBuilder.Entity<Document>(entity =>
        {
            entity.ToTable("Documents");
            entity.HasKey(d => d.Id);
            entity.Property(d => d.Id).HasDefaultValueSql("gen_random_uuid()");

            // Обязательные поля
            entity.Property(d => d.Title).IsRequired();
            entity.Property(d => d.LastEdited).HasDefaultValueSql("CURRENT_TIMESTAMP"); // Если это DateTime, используйте CURRENT_TIMESTAMP

            // Внешний ключ для владельца
            entity.HasOne(d => d.Owner)
                .WithMany(u => u.Documents)
                .HasForeignKey(d => d.OwnerId)
                .OnDelete(DeleteBehavior.Restrict); // Запретить удаление пользователя, если есть документы

            entity.HasMany(d => d.Reviewers).WithMany(d => d.ReviewDocuments);
        });
    }
}