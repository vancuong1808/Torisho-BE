using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Torisho.Domain.Entities.DictionaryDomain;

namespace Torisho.Infrastructure.Configurations.DictionaryDomain;

public sealed class DictionaryEntryDefinitionConfiguration : IEntityTypeConfiguration<DictionaryEntryDefinition>
{
    public void Configure(EntityTypeBuilder<DictionaryEntryDefinition> builder)
    {
        builder.ToTable("DictionaryEntryDefinitions");

        builder.HasKey(x => x.DictionaryEntryId);

        builder.Property(x => x.GlossText)
            .IsRequired()
            .HasColumnType("text");

        builder.HasIndex(x => x.DictionaryEntryId)
            .HasDatabaseName("IX_DictionaryEntryDefinitions_DictionaryEntryId");

        // NOTE: MySQL FULLTEXT index is created via migration SQL.
        // EF Core doesn't model FULLTEXT natively across providers.
    }
}
