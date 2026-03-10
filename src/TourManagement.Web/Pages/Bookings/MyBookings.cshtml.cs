using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Web.Pages.Bookings;

public class MyBookingsModel : PageModel
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<MyBookingsModel> _logger;

    public MyBookingsModel(IBookingService bookingService, ILogger<MyBookingsModel> logger)
    {
        _bookingService = bookingService;
        _logger = logger;
    }

    public IEnumerable<Booking> Bookings { get; set; } = new List<Booking>();
    public string? Email { get; set; }

    public async Task OnGetAsync()
    {
        Email = HttpContext.Session.GetString("UserEmail");

        if (!string.IsNullOrEmpty(Email))
        {
            try
            {
                Bookings = await _bookingService.GetBookingsByUserEmailAsync(Email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving bookings for user: {Email}", Email);
            }
        }
    }
}
