using AutoMapper;
using Microsoft.Extensions.Logging;
using TourManagement.Domain.DTOs;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Application.Services;

/// <summary>
/// Service implementation for Booking operations
/// </summary>
public class BookingService : IBookingService
{
    private readonly IBookingRepository _repository;
    private readonly ITourRepository _tourRepository;
    private readonly IUserInfoRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<BookingService> _logger;

    public BookingService(
        IBookingRepository repository,
        ITourRepository tourRepository,
        IUserInfoRepository userRepository,
        IMapper mapper,
        ILogger<BookingService> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _tourRepository = tourRepository ?? throw new ArgumentNullException(nameof(tourRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<BookingDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving all bookings");
            var bookings = await _repository.GetAllAsync(cancellationToken);
            return _mapper.Map<IEnumerable<BookingDto>>(bookings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all bookings");
            throw;
        }
    }

    public async Task<BookingDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving booking with id {BookingId}", id);
            var booking = await _repository.GetByIdAsync(id, cancellationToken);
            return booking == null ? null : _mapper.Map<BookingDto>(booking);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving booking with id {BookingId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<BookingDto>> GetByUserEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving bookings for user {Email}", email);
            var bookings = await _repository.GetByUserEmailAsync(email, cancellationToken);
            return _mapper.Map<IEnumerable<BookingDto>>(bookings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving bookings for user {Email}", email);
            throw;
        }
    }

    public async Task<IEnumerable<BookingDto>> GetByTourIdAsync(int tourId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving bookings for tour {TourId}", tourId);
            var bookings = await _repository.GetByTourIdAsync(tourId, cancellationToken);
            return _mapper.Map<IEnumerable<BookingDto>>(bookings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving bookings for tour {TourId}", tourId);
            throw;
        }
    }

    public async Task<BookingDto> CreateAsync(BookingCreateDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating new booking for tour {TourId} by user {Email}", dto.TourId, dto.Email);

            var tour = await _tourRepository.GetByIdAsync(dto.TourId, cancellationToken);
            if (tour == null)
                throw new InvalidOperationException($"Tour with id {dto.TourId} not found");

            var user = await _userRepository.GetByEmailAsync(dto.Email, cancellationToken);
            if (user == null)
                throw new InvalidOperationException($"User with email {dto.Email} not found");

            var booking = _mapper.Map<Booking>(dto);
            booking.TourName = tour.TourName;
            booking.Place = tour.Place;
            booking.FirstName = user.FirstName;

            var createdBooking = await _repository.AddAsync(booking, cancellationToken);

            _logger.LogInformation("Booking created successfully with id {BookingId}", createdBooking.Id);
            return _mapper.Map<BookingDto>(createdBooking);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating booking for tour {TourId} by user {Email}", dto.TourId, dto.Email);
            throw;
        }
    }

    public async Task<BookingDto> UpdateAsync(int id, BookingUpdateDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating booking with id {BookingId}", id);

            var existingBooking = await _repository.GetByIdAsync(id, cancellationToken);
            if (existingBooking == null)
            {
                _logger.LogWarning("Booking with id {BookingId} not found", id);
                throw new InvalidOperationException($"Booking with id {id} not found");
            }

            var tour = await _tourRepository.GetByIdAsync(dto.TourId, cancellationToken);
            if (tour == null)
                throw new InvalidOperationException($"Tour with id {dto.TourId} not found");

            var user = await _userRepository.GetByEmailAsync(dto.Email, cancellationToken);
            if (user == null)
                throw new InvalidOperationException($"User with email {dto.Email} not found");

            _mapper.Map(dto, existingBooking);
            existingBooking.Id = id;
            existingBooking.TourName = tour.TourName;
            existingBooking.Place = tour.Place;
            existingBooking.FirstName = user.FirstName;

            var updatedBooking = await _repository.UpdateAsync(existingBooking, cancellationToken);

            _logger.LogInformation("Booking with id {BookingId} updated successfully", id);
            return _mapper.Map<BookingDto>(updatedBooking);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating booking with id {BookingId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting booking with id {BookingId}", id);

            var exists = await _repository.ExistsAsync(id, cancellationToken);
            if (!exists)
            {
                _logger.LogWarning("Booking with id {BookingId} not found", id);
                return false;
            }

            var result = await _repository.DeleteAsync(id, cancellationToken);

            if (result)
                _logger.LogInformation("Booking with id {BookingId} deleted successfully", id);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting booking with id {BookingId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<BookingDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Searching bookings with term: {SearchTerm}", searchTerm);
            var bookings = await _repository.SearchAsync(searchTerm, cancellationToken);
            return _mapper.Map<IEnumerable<BookingDto>>(bookings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching bookings with term: {SearchTerm}", searchTerm);
            throw;
        }
    }
}
