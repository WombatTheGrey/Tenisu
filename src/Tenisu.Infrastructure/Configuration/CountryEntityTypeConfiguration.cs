using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tenisu.Domain.Entities;

namespace Tenisu.Infrastructure.Configuration
{
    internal class CountryEntityTypeConfiguration : IEntityTypeConfiguration<Country>
    {
        public void Configure(EntityTypeBuilder<Country> builder)
        {
            builder.ToTable("Countries")
                .HasKey(c => c.Code);

            builder.Property(c => c.Code)
                .IsRequired()
                .HasMaxLength(3);

            builder.Property(c => c.Picture)
                .IsRequired()
                .HasConversion<string>();
        }
    }
}
