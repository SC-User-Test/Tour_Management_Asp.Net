using Microsoft.Extensions.Logging;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Exceptions;
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Application.Services;

/// <summary>
/// Service implementation for tour operations
/// </summary>
public class TourService : ITourService
{
    private readonly ITourRepository _tourRepository;
    private readonly ILogger<TourService> _logger;

    public TourService(ITourRepository tourRepository, ILogger<TourService> logger)
    {
        _tourRepository = tourRepository ?? throw new ArgumentNullException(nameof(tourRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<Tour>> GetAllToursAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving all tours");
            return await _tourRepository.GetAllAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all tours");
            throw new TourManagementException("Error retrieving tours", ex);
        }
    }

    public async Task<Tour?> GetTourByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving tour with ID: {TourId}", id);
            return await _tourRepository.GetByIdAsync(id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tour with ID: {TourId}", id);
            throw new TourManagementException($"Error retrieving tour with ID: {id}", ex);
        }
    }

    public async Task<int> CreateTourAsync(Tour tour, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating new tour: {TourName}", tour.TourName);

            if (string.IsNullOrWhiteSpace(tour.TourName))
            {
                throw new ValidationException("Tour name is required");
            }

            tour.CreatedDate = DateTime.UtcNow;
            tour.IsActive = true;

            var tourId = await _tourRepository.AddAsync(tour, cancellationToken);

            _logger.LogInformation("Tour created successfully with ID: {TourId}", tourId);
            return tourId;
        }
        catch (ValidationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating tour: {TourName}", tour.TourName);
            throw new TourManagementException($"Error creating tour: {tour.TourName}", ex);
        }
    }

    public async Task UpdateTourAsync(int id, Tour tour, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating tour with ID: {TourId}", id);

            var existingTour = await _tourRepository.GetByIdAsync(id, cancellationToken);
            if (existingTour == null)
            {
                throw new EntityNotFoundException(nameof(Tour), id);
            }

            tour.TourId = id;
            tour.ModifiedDate = DateTime.UtcNow;

            await _tourRepository.UpdateAsync(tour, cancellationToken);

            _logger.LogInformation("Tour updated successfully with ID: {TourId}", id);
        }
        catch (EntityNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tour with ID: {TourId}", id);
            throw new TourManagementException($"Error updating tour with ID: {id}", ex);
        }
    }

    public async Task DeleteTourAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting tour with ID: {TourId}", id);

            var existingTour = await _tourRepository.GetByIdAsync(id, cancellationToken);
            if (existingTour == null)
            {
                throw new EntityNotFoundException(nameof(Tour), id);
            }

            await _tourRepository.DeleteAsync(id, cancellationToken);

            _logger.LogInformation("Tour deleted successfully with ID: {TourId}", id);
        }
        catch (EntityNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting tour with ID: {TourId}", id);
            throw new TourManagementException($"Error deleting tour with ID: {id}", ex);
        }
    }

    public async Task<IEnumerable<Tour>> SearchToursAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Searching tours with term: {SearchTerm}", searchTerm);
            return await _tourRepository.SearchAsync(searchTerm, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching tours with term: {SearchTerm}", searchTerm);
            throw new TourManagementException($"Error searching tours with term: {searchTerm}", ex);
        }
    }
}
