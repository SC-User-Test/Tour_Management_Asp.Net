namespace TourManagement.Domain.Entities;

/// <summary>
/// Represents a user entity
/// </summary>
public class UserInfo
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public bool IsActive { get; set; }
    public string? ProfilePictureFileName { get; set; }

    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
