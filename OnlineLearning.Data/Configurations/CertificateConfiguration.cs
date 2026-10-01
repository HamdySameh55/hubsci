using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineLearning.Domain.Entities;

namespace OnlineLearning.Data.Configurations;

public class CertificateConfiguration : IEntityTypeConfiguration<Certificate>
{
    public void Configure(EntityTypeBuilder<Certificate> builder)
    {
        builder.HasKey(c => c.CertificateId);

        builder.Property(c => c.CertificateUrl)
            .IsRequired()
            .HasMaxLength(500);

        builder.HasOne(c => c.Enrollment)
            .WithOne(e => e.Certificate)
            .HasForeignKey<Certificate>(c => c.EnrollmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(c => c.EnrollmentId)
            .IsUnique();
    }
}