using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechStore.Domain.Entities;

namespace TechStore.DAL.Configurations;

public class HomepageSectionItemConfiguration : IEntityTypeConfiguration<HomepageSectionItem>
{
    public void Configure(EntityTypeBuilder<HomepageSectionItem> builder)
    {
        builder.ToTable("HomepageSectionItems");

        builder.HasOne(item => item.HomepageSection)
            .WithMany(section => section.Items)
            .HasForeignKey(item => item.HomepageSectionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(item => item.Product)
            .WithMany(product => product.HomepageSectionItems)
            .HasForeignKey(item => item.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
