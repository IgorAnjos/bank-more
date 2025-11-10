using BankMore.ContaCorrente.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankMore.ContaCorrente.Infrastructure.Data.Configurations;

public class OutboxEventConfiguration : IEntityTypeConfiguration<OutboxEvent>
{
    public void Configure(EntityTypeBuilder<OutboxEvent> builder)
    {
        builder.ToTable("outbox_events");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasColumnType("TEXT(37)")
            .IsRequired();

        builder.Property(x => x.Topic)
            .HasColumnName("topic")
            .HasColumnType("TEXT(100)")
            .IsRequired();

        builder.Property(x => x.EventType)
            .HasColumnName("event_type")
            .HasColumnType("TEXT(100)")
            .IsRequired();

        builder.Property(x => x.Payload)
            .HasColumnName("payload")
            .HasColumnType("TEXT")
            .IsRequired();

        builder.Property(x => x.PartitionKey)
            .HasColumnName("partition_key")
            .HasColumnType("TEXT(100)");

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(x => x.ProcessedAt)
            .HasColumnName("processed_at");

        builder.Property(x => x.Processed)
            .HasColumnName("processed")
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(x => x.RetryCount)
            .HasColumnName("retry_count")
            .HasDefaultValue(0)
            .IsRequired();

        builder.Property(x => x.ErrorMessage)
            .HasColumnName("error_message")
            .HasColumnType("TEXT(500)");

        // Índices para performance
        builder.HasIndex(x => new { x.Processed, x.CreatedAt })
            .HasDatabaseName("IX_outbox_events_processed_created_at");

        builder.HasIndex(x => x.Topic)
            .HasDatabaseName("IX_outbox_events_topic");
    }
}
