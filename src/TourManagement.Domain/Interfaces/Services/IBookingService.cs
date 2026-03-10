using TourManagement.Domain.Entities;

namespace TourManagement.Domain.Interfaces.Services;

/// <summary>
/// Service interface for booking operations
/// </summary>
public interface IBookingService
{
    Task<IEnumerable<Booking>> GetAllBookingsAsync(CancellationToken cancellationToken = default);
    Task<Booking?> GetBookingByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Booking>> GetBookingsByUserEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<int> CreateBookingAsync(Booking booking, CancellationToken cancellationToken = default);
    Task UpdateBookingAsync(int id, Booking booking, CancellationToken cancellationToken = default);
    Task DeleteBookingAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Booking>> SearchBookingsAsync(string searchTerm, CancellationToken cancellationToken = default);
}
