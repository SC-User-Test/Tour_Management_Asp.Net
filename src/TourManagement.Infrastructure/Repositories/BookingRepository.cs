using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Infrastructure.Data;

namespace TourManagement.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Booking entity
/// </summary>
public class BookingRepository : IBookingRepository
{
    private readonly TourManagementDbContext _context;
    private readonly ILogger<BookingRepository> _logger;

    public BookingRepository(TourManagementDbContext context, ILogger<BookingRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<Booking>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Bookings
            .Include(b => b.Tour)
            .Include(b => b.User)
            .Where(b => b.IsActive)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Booking?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Bookings
            .Include(b => b.Tour)
            .Include(b => b.User)
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.BookingId == id, cancellationToken);
    }

    public async Task<IEnumerable<Booking>> GetByUserEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Bookings
            .Include(b => b.Tour)
            .Where(b => b.Email == email && b.IsActive)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<int> AddAsync(Booking booking, CancellationToken cancellationToken = default)
    {
        await _context.Bookings.AddAsync(booking, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return booking.BookingId;
    }

    public async Task UpdateAsync(Booking booking, CancellationToken cancellationToken = default)
    {
        _context.Bookings.Update(booking);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var booking = await _context.Bookings.FindAsync(new object[] { id }, cancellationToken);
        if (booking != null)
        {
            booking.IsActive = false;
            booking.ModifiedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Bookings.AnyAsync(b => b.BookingId == id, cancellationToken);
    }

    public async Task<IEnumerable<Booking>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        return await _context.Bookings
            .Include(b => b.Tour)
            .Where(b => b.IsActive &&
                (b.TourName.Contains(searchTerm) ||
                 b.Email.Contains(searchTerm) ||
                 b.FirstName.Contains(searchTerm)))
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}
