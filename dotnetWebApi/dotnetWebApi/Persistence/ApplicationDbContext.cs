using dotnetWebApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace dotnetWebApi.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
        : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Document> Documents { get; set; }
    public DbSet<Reviewer> Reviewers { get; set; }
    public DbSet<Comment> Comments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Id).HasDefaultValueSql("gen_random_uuid()");

            entity.Property(u => u.UserName)
                .IsRequired()
                .HasMaxLength(256);

            entity.Property(u => u.FirstName).IsRequired();
            entity.Property(u => u.Role).IsRequired();
            entity.Property(u => u.PasswordHash).IsRequired();

            entity.HasMany(u => u.Documents)
                .WithOne(d => d.Owner)
                .HasForeignKey(d => d.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

        });

        modelBuilder.Entity<Document>(entity =>
        {
            entity.ToTable("Documents");
            entity.HasKey(d => d.Id);
            entity.Property(d => d.Id).HasDefaultValueSql("gen_random_uuid()");

            // Обязательные поля
            entity.Property(d => d.Title).IsRequired();
            entity.Property(d => d.LastModified).HasDefaultValueSql("CURRENT_TIMESTAMP"); 
        });
        
        modelBuilder.Entity<Reviewer>(entity =>
        {
            entity.ToTable("Reviewers");
            entity.HasKey(d => d.Id);
            entity.Property(d => d.Id).HasDefaultValueSql("gen_random_uuid()");

            entity.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Document)
                .WithMany(x => x.Reviewers)
                .HasForeignKey(x => x.DocumentId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.ToTable("Comments");
            entity.HasKey(d => d.Id);
            entity.Property(d => d.Id).HasDefaultValueSql("gen_random_uuid()");
            
            entity.HasOne(x => x.Document)
                .WithMany()
                .HasForeignKey(x => x.DocumentId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(x => x.Reviewer)
                .WithMany()
                .HasForeignKey(x => x.ReviewerId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}