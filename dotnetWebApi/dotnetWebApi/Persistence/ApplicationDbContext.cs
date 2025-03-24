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
            // Id is considered a key by default
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Id).HasDefaultValueSql("gen_random_uuid()");

            entity.Property(u => u.UserName)
                .IsRequired()
                .HasMaxLength(256);

            // I'm not 100% sure, but I believe that IsRequired can be replaced by just using the required c# keyword
            entity.Property(u => u.FirstName).IsRequired();
            entity.Property(u => u.Role).IsRequired();
            entity.Property(u => u.PasswordHash).IsRequired();

            // As mentioned in one of the entities, you don't have to do this if you mark property as virtual
            entity.HasMany(u => u.OwnedDocuments)
                .WithOne(d => d.Owner)
                .HasForeignKey(d => d.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Same comments apply to other entities
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
                .WithMany(x => x.ReviewAssignments)
                .HasForeignKey(x => x.UserId)
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
            entity.Property(c => c.ParentCommentId).HasColumnName("ParentCommentId").IsRequired(false);
            
            entity.HasOne(x => x.Document)
                .WithMany(x => x.Comments)
                .HasForeignKey(x => x.DocumentId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(x => x.Reviewer)
                .WithMany(x => x.Comments)
                .HasForeignKey(x => x.ReviewerId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(x => x.ParentComment)
                .WithMany(x => x.Replies)
                .HasForeignKey(x => x.ParentCommentId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasIndex(x => x.ParentCommentId);
        });
    }
}