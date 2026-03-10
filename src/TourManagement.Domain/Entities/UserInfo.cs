using TourManagement.Domain.Enums;

namespace TourManagement.Domain.Entities;

/// <summary>
/// Represents a user in the tour management system
/// </summary>
public class UserInfo
{
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public Gender Gender { get; set; }
    public string Password { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public bool IsActive { get; set; } = true;
    public string CreatedBy { get; set; } = "System";
    public string? ModifiedBy { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
