using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TourManagement.Domain.Entities;

namespace TourManagement.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for Booking entity
/// </summary>
public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("Booking");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Id)
            .HasColumnName("BOOKING_ID");

        builder.Property(b => b.TourId)
            .IsRequired()
            .HasColumnName("TOUR_ID");

        builder.Property(b => b.UserId)
            .IsRequired()
            .HasColumnName("USER_ID");

        builder.Property(b => b.CustomerName)
            .IsRequired()
            .HasMaxLength(200)
            .HasColumnName("CUSTOMER_NAME");

        builder.Property(b => b.CustomerEmail)
            .IsRequired()
            .HasMaxLength(255)
            .HasColumnName("CUSTOMER_EMAIL");

        builder.Property(b => b.CustomerPhone)
            .HasMaxLength(20)
            .HasColumnName("CUSTOMER_PHONE");

        builder.Property(b => b.NumberOfPeople)
            .IsRequired()
            .HasColumnName("NUMBER_OF_PEOPLE");

        builder.Property(b => b.BookingDate)
            .IsRequired()
            .HasColumnName("BOOKING_DATE");

        builder.Property(b => b.TravelDate)
            .IsRequired()
            .HasColumnName("TRAVEL_DATE");

        builder.Property(b => b.TotalAmount)
            .IsRequired()
            .HasColumnType("decimal(18,2)")
            .HasColumnName("TOTAL_AMOUNT");

        builder.Property(b => b.Status)
            .HasMaxLength(50)
            .HasColumnName("STATUS")
            .HasDefaultValue("Pending");

        builder.Property(b => b.Notes)
            .HasMaxLength(1000)
            .HasColumnName("NOTES");

        builder.Property(b => b.CreatedDate)
            .HasDefaultValueSql("NOW()");

        builder.Property(b => b.IsActive)
            .HasDefaultValue(true);

        builder.HasOne(b => b.Tour)
            .WithMany(t => t.Bookings)
            .HasForeignKey(b => b.TourId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.User)
            .WithMany(u => u.Bookings)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
