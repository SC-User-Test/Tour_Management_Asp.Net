using AutoMapper;
using Microsoft.Extensions.Logging;
using TourManagement.Domain.DTOs;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Application.Services;

/// <summary>
/// Service implementation for Tour operations
/// </summary>
public class TourService : ITourService
{
    private readonly ITourRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<TourService> _logger;

    public TourService(ITourRepository repository, IMapper mapper, ILogger<TourService> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<TourDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving all tours");
            var tours = await _repository.GetAllAsync(cancellationToken);
            return _mapper.Map<IEnumerable<TourDto>>(tours);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all tours");
            throw;
        }
    }

    public async Task<TourDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving tour with id {TourId}", id);
            var tour = await _repository.GetByIdAsync(id, cancellationToken);
            return tour == null ? null : _mapper.Map<TourDto>(tour);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tour with id {TourId}", id);
            throw;
        }
    }

    public async Task<TourDto> CreateAsync(TourCreateDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating new tour: {TourName}", dto.TourName);

            if (string.IsNullOrWhiteSpace(dto.TourName))
                throw new ArgumentException("Tour name is required", nameof(dto.TourName));

            var tour = _mapper.Map<Tour>(dto);
            var createdTour = await _repository.AddAsync(tour, cancellationToken);

            _logger.LogInformation("Tour created successfully with id {TourId}", createdTour.Id);
            return _mapper.Map<TourDto>(createdTour);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating tour: {TourName}", dto.TourName);
            throw;
        }
    }

    public async Task<TourDto> UpdateAsync(int id, TourUpdateDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating tour with id {TourId}", id);

            var existingTour = await _repository.GetByIdAsync(id, cancellationToken);
            if (existingTour == null)
            {
                _logger.LogWarning("Tour with id {TourId} not found", id);
                throw new InvalidOperationException($"Tour with id {id} not found");
            }

            _mapper.Map(dto, existingTour);
            existingTour.Id = id;

            var updatedTour = await _repository.UpdateAsync(existingTour, cancellationToken);

            _logger.LogInformation("Tour with id {TourId} updated successfully", id);
            return _mapper.Map<TourDto>(updatedTour);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tour with id {TourId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting tour with id {TourId}", id);

            var exists = await _repository.ExistsAsync(id, cancellationToken);
            if (!exists)
            {
                _logger.LogWarning("Tour with id {TourId} not found", id);
                return false;
            }

            var result = await _repository.DeleteAsync(id, cancellationToken);

            if (result)
                _logger.LogInformation("Tour with id {TourId} deleted successfully", id);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting tour with id {TourId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<TourDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Searching tours with term: {SearchTerm}", searchTerm);
            var tours = await _repository.SearchAsync(searchTerm, cancellationToken);
            return _mapper.Map<IEnumerable<TourDto>>(tours);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching tours with term: {SearchTerm}", searchTerm);
            throw;
        }
    }
}
