using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechStore.Domain.Entities;

namespace TechStore.DAL.Configurations;

public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.ToTable("Addresses");

        builder.Property(address => address.Label).HasMaxLength(100).IsRequired();
        builder.Property(address => address.StreetAddress).HasMaxLength(300).IsRequired();
        builder.Property(address => address.City).HasMaxLength(100).IsRequired();
        builder.Property(address => address.Governorate).HasMaxLength(100).IsRequired();
        builder.Property(address => address.PhoneNumber).HasMaxLength(20).IsRequired();

        builder.HasOne(address => address.User)
            .WithMany(user => user.Addresses)
            .HasForeignKey(address => address.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
