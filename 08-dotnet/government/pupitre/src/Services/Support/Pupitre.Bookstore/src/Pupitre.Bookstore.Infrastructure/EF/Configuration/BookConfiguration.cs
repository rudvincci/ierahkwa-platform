using System;
using Pupitre.Bookstore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Pupitre.Bookstore.Infrastructure.EF.Configuration;

public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public BookConfiguration()
    {
    }

    void IEntityTypeConfiguration<Book>.Configure(EntityTypeBuilder<Book> builder)
    {
        builder.ToTable("book");

        // Primary key
        builder.HasKey(c => c.Id);

        // Properties
        builder.Property(c => c.Id)
        .HasConversion(c=> c.Value, c=> new BookId(c))
        .IsRequired();

        // Tags Collection (assuming many-to-many or storing as a serialized array)
        builder.Property(c => c.Tags)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries))
            .IsRequired(false);

        builder.Ignore(c => c.Events);
        // Indexes
        builder.HasIndex(c => c.Name)
            .IsUnique(false);
    }
}

