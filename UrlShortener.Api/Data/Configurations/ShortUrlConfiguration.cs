using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrlShortener.Api.Domain.Entities;

namespace UrlShortener.Api.Data.Configurations;

public class ShortUrlConfiguration : IEntityTypeConfiguration<ShortUrl>
{
    public void Configure(EntityTypeBuilder<ShortUrl> builder)
    {
        builder.ToTable("ShortUrls");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ShortCode)
            .IsRequired()
            .HasMaxLength(10);

        builder.HasIndex(x => x.ShortCode)
            .IsUnique();

        builder.Property(x => x.LongUrl)
            .IsRequired()
            .HasMaxLength(2048);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.Property(x => x.ClickCount)
            .IsRequired();

        builder.HasMany(x => x.Clicks)
            .WithOne(x => x.ShortUrl)
            .HasForeignKey(x => x.ShortUrlId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}