using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Torisho.Domain.Entities.DictionaryDomain;

namespace Torisho.Infrastructure.Configurations.DictionaryDomain;

public sealed class DictionaryEntryDefinitionConfiguration : IEntityTypeConfiguration<DictionaryEntryDefinition>
{
    public void Configure(EntityTypeBuilder<DictionaryEntryDefinition> builder)
    {
        builder.ToTable("entry_definitions");

        builder.HasKey(x => x.DictionaryEntryId);

        builder.Property(x => x.DictionaryEntryId)
            .HasColumnName("entry_id");

        builder.Property(x => x.GlossText)
            .IsRequired()
            .HasColumnName("gloss_text")
            .HasColumnType("text");

        builder.HasIndex(x => x.DictionaryEntryId)
            .HasDatabaseName("ix_entry_definitions_entry_id");

        // NOTE: MySQL FULLTEXT index is created via migration SQL.
        // EF Core doesn't model FULLTEXT natively across providers.
    }
}
