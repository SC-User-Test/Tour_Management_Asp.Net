using Microsoft.Extensions.Logging;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Exceptions;
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Application.Services;

/// <summary>
/// Service implementation for booking operations
/// </summary>
public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly ILogger<BookingService> _logger;

    public BookingService(IBookingRepository bookingRepository, ILogger<BookingService> logger)
    {
        _bookingRepository = bookingRepository ?? throw new ArgumentNullException(nameof(bookingRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<Booking>> GetAllBookingsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving all bookings");
            return await _bookingRepository.GetAllAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all bookings");
            throw new TourManagementException("Error retrieving bookings", ex);
        }
    }

    public async Task<Booking?> GetBookingByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving booking with ID: {BookingId}", id);
            return await _bookingRepository.GetByIdAsync(id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving booking with ID: {BookingId}", id);
            throw new TourManagementException($"Error retrieving booking with ID: {id}", ex);
        }
    }

    public async Task<IEnumerable<Booking>> GetBookingsByUserEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving bookings for user: {Email}", email);
            return await _bookingRepository.GetByUserEmailAsync(email, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving bookings for user: {Email}", email);
            throw new TourManagementException($"Error retrieving bookings for user: {email}", ex);
        }
    }

    public async Task<int> CreateBookingAsync(Booking booking, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating new booking for tour: {TourName}", booking.TourName);

            if (string.IsNullOrWhiteSpace(booking.Email))
            {
                throw new ValidationException("Email is required");
            }

            booking.CreatedDate = DateTime.UtcNow;
            booking.BookingDate = DateTime.UtcNow;
            booking.IsActive = true;

            var bookingId = await _bookingRepository.AddAsync(booking, cancellationToken);

            _logger.LogInformation("Booking created successfully with ID: {BookingId}", bookingId);
            return bookingId;
        }
        catch (ValidationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating booking for tour: {TourName}", booking.TourName);
            throw new TourManagementException($"Error creating booking for tour: {booking.TourName}", ex);
        }
    }

    public async Task UpdateBookingAsync(int id, Booking booking, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating booking with ID: {BookingId}", id);

            var existingBooking = await _bookingRepository.GetByIdAsync(id, cancellationToken);
            if (existingBooking == null)
            {
                throw new EntityNotFoundException(nameof(Booking), id);
            }

            booking.BookingId = id;
            booking.ModifiedDate = DateTime.UtcNow;

            await _bookingRepository.UpdateAsync(booking, cancellationToken);

            _logger.LogInformation("Booking updated successfully with ID: {BookingId}", id);
        }
        catch (EntityNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating booking with ID: {BookingId}", id);
            throw new TourManagementException($"Error updating booking with ID: {id}", ex);
        }
    }

    public async Task DeleteBookingAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting booking with ID: {BookingId}", id);

            var existingBooking = await _bookingRepository.GetByIdAsync(id, cancellationToken);
            if (existingBooking == null)
            {
                throw new EntityNotFoundException(nameof(Booking), id);
            }

            await _bookingRepository.DeleteAsync(id, cancellationToken);

            _logger.LogInformation("Booking deleted successfully with ID: {BookingId}", id);
        }
        catch (EntityNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting booking with ID: {BookingId}", id);
            throw new TourManagementException($"Error deleting booking with ID: {id}", ex);
        }
    }

    public async Task<IEnumerable<Booking>> SearchBookingsAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Searching bookings with term: {SearchTerm}", searchTerm);
            return await _bookingRepository.SearchAsync(searchTerm, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching bookings with term: {SearchTerm}", searchTerm);
            throw new TourManagementException($"Error searching bookings with term: {searchTerm}", ex);
        }
    }
}
