using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineLearning.Domain.Entities;

namespace OnlineLearning.Data.Configurations;

public class FeedbackConfiguration : IEntityTypeConfiguration<Feedback>
{
    public void Configure(EntityTypeBuilder<Feedback> builder)
    {
        builder.HasKey(f => f.FeedbackId);

        builder.Property(f => f.Comment)
            .HasMaxLength(1000);

        builder.Property(f => f.Rating)
            .IsRequired();

        builder.Property(f => f.Status)
            .IsRequired()
            .HasMaxLength(30);

        builder.HasOne(f => f.User)
            .WithMany(u => u.Feedbacks)
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(f => f.Course)
            .WithMany(c => c.Feedbacks)
            .HasForeignKey(f => f.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        // One feedback per student per course
        builder.HasIndex(f => new { f.UserId, f.CourseId })
            .IsUnique();
    }
}