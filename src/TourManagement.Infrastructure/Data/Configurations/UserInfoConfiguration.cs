using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Enums;

namespace TourManagement.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for UserInfo
/// </summary>
public class UserInfoConfiguration : IEntityTypeConfiguration<UserInfo>
{
    public void Configure(EntityTypeBuilder<UserInfo> builder)
    {
        builder.ToTable("UserInfo");

        builder.HasKey(u => u.Email);

        builder.Property(u => u.Email)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(u => u.FirstName)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(u => u.LastName)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(u => u.Gender)
            .HasConversion(
                v => v.ToString(),
                v => (Gender)Enum.Parse(typeof(Gender), v))
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(u => u.Password)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(u => u.DateOfBirth)
            .HasColumnName("dob")
            .IsRequired();

        builder.Property(u => u.Street)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(u => u.City)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(u => u.State)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(u => u.CreatedDate)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(u => u.IsActive)
            .HasDefaultValue(true);

        builder.HasMany(u => u.Bookings)
            .WithOne(b => b.User)
            .HasForeignKey(b => b.Email)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
