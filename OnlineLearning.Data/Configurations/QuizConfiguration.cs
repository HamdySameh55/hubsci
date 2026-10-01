using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineLearning.Domain.Entities;

namespace OnlineLearning.Data.Configurations;

public class QuizConfiguration : IEntityTypeConfiguration<Quiz>
{
    public void Configure(EntityTypeBuilder<Quiz> builder)
    {
        builder.HasKey(q => q.QuizId);

        builder.Property(q => q.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(q => q.Description)
            .IsRequired();

        builder.HasOne(q => q.Module)
            .WithOne(m => m.Quiz)
            .HasForeignKey<Quiz>(q => q.ModuleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(q => q.ModuleId)
            .IsUnique();
    }
}