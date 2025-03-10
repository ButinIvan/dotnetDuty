using dotnetWebApi.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace dotnetWebApi.Persistence.Сonfigurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(u => u.UserName).IsRequired().HasMaxLength(64);
        builder.Property(u => u.FirstName).IsRequired();
        builder.Property(u => u.Role).IsRequired();
        builder.Property(u => u.PasswordHash).IsRequired();

        builder.HasIndex(u => u.UserName).IsUnique();
        
        builder.HasMany(u => u.Documents)
            .WithOne(d => d.Owner) 
            .HasForeignKey(d => d.OwnerId) 
            .OnDelete(DeleteBehavior.Cascade) 
            .IsRequired();

        builder.HasMany(u => u.ReviewDocuments)
            .WithMany(d => d.Reviewers);
    }
}