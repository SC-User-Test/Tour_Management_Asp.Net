using TourManagement.Domain.Entities;

namespace TourManagement.Domain.Interfaces.Services;

/// <summary>
/// Service interface for tour operations
/// </summary>
public interface ITourService
{
    Task<IEnumerable<Tour>> GetAllToursAsync(CancellationToken cancellationToken = default);
    Task<Tour?> GetTourByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<int> CreateTourAsync(Tour tour, CancellationToken cancellationToken = default);
    Task UpdateTourAsync(int id, Tour tour, CancellationToken cancellationToken = default);
    Task DeleteTourAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Tour>> SearchToursAsync(string searchTerm, CancellationToken cancellationToken = default);
}
