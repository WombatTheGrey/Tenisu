using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tenisu.Domain.Entities;

namespace Tenisu.Infrastructure.Configuration
{
    internal class PlayerEntityTypeConfiguration : IEntityTypeConfiguration<Player>
    {
        public void Configure(EntityTypeBuilder<Player> builder)
        {
            builder.ToTable("Players")
                .Ignore(p => p.ShortName)
                .HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .ValueGeneratedOnAdd();

            builder.Property(p => p.FirstName)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(p => p.LastName)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(p => p.Sex)
                .IsRequired()
                .HasConversion<string>();

            builder.HasIndex(p => new { p.FirstName, p.LastName, p.Sex })
                .IsUnique();

            builder.HasOne(p=> p.Country)
                .WithMany();

            builder.Property(p => p.Picture)
                .IsRequired()
                .HasConversion<string>();

            builder.OwnsOne(p => p.Data, data =>
            {
                data.HasIndex(d => d.Rank)
                    .IsUnique();
                data.Property(d => d.Rank)
                    .IsRequired();
                data.Property(d => d.Points)
                    .IsRequired();
                data.Property(d => d.Weight)
                    .IsRequired();
                data.Property(d => d.Height)
                    .IsRequired();
                data.Property(d => d.Age)
                    .IsRequired();
                data.Property(d => d.Last)
                    .IsRequired();
            });
        }
    }

    
}
