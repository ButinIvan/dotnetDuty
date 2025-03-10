using dotnetWebApi.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace dotnetWebApi.Persistence.Сonfigurations;

public class DocumentConfiguration : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(u => u.Title).IsRequired().HasMaxLength(64);
        builder.Property(u => u.OwnerId).IsRequired();
        builder.Property(u => u.IsFinished).IsRequired();
        builder.Property(u => u.IsClosedToComment).IsRequired();
        builder.Property(u => u.LastEdited).IsRequired();
        builder.Property(u => u.S3Path).IsRequired().HasMaxLength(512);
    }
}