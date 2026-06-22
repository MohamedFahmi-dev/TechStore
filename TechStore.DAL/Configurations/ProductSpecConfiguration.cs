using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechStore.Domain.Entities;

namespace TechStore.DAL.Configurations;

public class ProductSpecConfiguration : IEntityTypeConfiguration<ProductSpec>
{
    public void Configure(EntityTypeBuilder<ProductSpec> builder)
    {
        builder.ToTable("ProductSpecs");

        builder.Property(spec => spec.GroupName).HasMaxLength(100).IsRequired();
        builder.Property(spec => spec.Name).HasMaxLength(100).IsRequired();
        builder.Property(spec => spec.Value).HasMaxLength(500).IsRequired();

        builder.HasOne(spec => spec.Product)
            .WithMany(product => product.Specs)
            .HasForeignKey(spec => spec.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
