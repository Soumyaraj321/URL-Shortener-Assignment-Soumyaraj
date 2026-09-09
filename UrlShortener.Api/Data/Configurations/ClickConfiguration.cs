using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrlShortener.Api.Domain.Entities;

namespace UrlShortener.Api.Data.Configurations;

public class ClickConfiguration : IEntityTypeConfiguration<Click>
{
    public void Configure(EntityTypeBuilder<Click> builder)
    {
        builder.ToTable("Clicks");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.AccessedAt)
            .IsRequired();

        builder.Property(x => x.UserAgent)
            .HasMaxLength(1000);

        builder.Property(x => x.Referrer)
            .HasMaxLength(2048);

        builder.HasIndex(x => x.ShortUrlId);

        builder.HasIndex(x => x.AccessedAt);
    }
}