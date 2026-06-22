using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechStore.Domain.Entities;

namespace TechStore.DAL.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");

        builder.Property(category => category.Name).HasMaxLength(150).IsRequired();
        builder.Property(category => category.Slug).HasMaxLength(150).IsRequired();
        builder.Property(category => category.Description).HasMaxLength(1000);
        builder.Property(category => category.ImageUrl).HasMaxLength(500);

        builder.HasIndex(category => category.Slug).IsUnique();

        builder.HasOne(category => category.ParentCategory)
            .WithMany(parent => parent.SubCategories)
            .HasForeignKey(category => category.ParentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
