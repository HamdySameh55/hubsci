using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineLearning.Domain.Entities;

namespace OnlineLearning.Data.Configurations;

public class LessonProgressConfiguration : IEntityTypeConfiguration<LessonProgress>
{
    public void Configure(EntityTypeBuilder<LessonProgress> builder)
    {
        builder.HasKey(p => p.ProgressId);

        builder.HasOne(p => p.Enrollment)
            .WithMany(e => e.LessonProgresses)
            .HasForeignKey(p => p.EnrollmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.Lesson)
            .WithMany(l => l.ProgressRecords)
            .HasForeignKey(p => p.LessonId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => new { p.EnrollmentId, p.LessonId })
            .IsUnique();
    }
}