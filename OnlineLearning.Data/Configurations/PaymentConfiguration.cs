using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineLearning.Domain.Entities;

namespace OnlineLearning.Data.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.HasKey(p => p.PaymentId);

        builder.Property(p => p.Amount)
            .HasPrecision(18, 2);

        builder.Property(p => p.PaymentStatus)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(p => p.PaymentMethod)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.TransactionReference)
            .HasMaxLength(100);

        builder.Property(p => p.ProofFilePath)
            .HasMaxLength(300);

        // Enrollment 1 : 1 Payment
        builder.HasIndex(p => p.EnrollmentId)
            .IsUnique();

        builder.HasOne(p => p.Enrollment)
            .WithOne(e => e.Payment)
            .HasForeignKey<Payment>(p => p.EnrollmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}