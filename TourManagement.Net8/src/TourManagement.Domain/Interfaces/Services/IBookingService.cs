using TourManagement.Domain.DTOs;

namespace TourManagement.Domain.Interfaces.Services;

/// <summary>
/// Service interface for Booking operations
/// </summary>
public interface IBookingService
{
    Task<IEnumerable<BookingDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<BookingDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<BookingDto>> GetByUserEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<BookingDto>> GetByTourIdAsync(int tourId, CancellationToken cancellationToken = default);
    Task<BookingDto> CreateAsync(BookingCreateDto dto, CancellationToken cancellationToken = default);
    Task<BookingDto> UpdateAsync(int id, BookingUpdateDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<BookingDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
}
