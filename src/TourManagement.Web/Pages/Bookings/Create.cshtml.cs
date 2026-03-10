using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Web.Pages.Bookings;

public class BookingsCreateModel : PageModel
{
    private readonly IBookingService _bookingService;
    private readonly ITourService _tourService;
    private readonly ILogger<BookingsCreateModel> _logger;

    public BookingsCreateModel(IBookingService bookingService, ITourService tourService, ILogger<BookingsCreateModel> logger)
    {
        _bookingService = bookingService;
        _tourService = tourService;
        _logger = logger;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new InputModel();

    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }

    public class InputModel
    {
        public int TourId { get; set; }

        [Required]
        public string TourName { get; set; } = string.Empty;

        [Required]
        public string Place { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;
    }

    public async Task<IActionResult> OnGetAsync(int? tourId)
    {
        if (tourId == null)
        {
            return NotFound();
        }

        try
        {
            var tour = await _tourService.GetTourByIdAsync(tourId.Value);
            if (tour == null)
            {
                return NotFound();
            }

            Input.TourId = tour.TourId;
            Input.TourName = tour.TourName;
            Input.Place = tour.Place;

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading tour for booking: {TourId}", tourId);
            return NotFound();
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var booking = new Booking
            {
                TourId = Input.TourId,
                TourName = Input.TourName,
                Place = Input.Place,
                Email = Input.Email,
                FirstName = Input.FirstName
            };

            await _bookingService.CreateBookingAsync(booking);

            _logger.LogInformation("Booking created successfully for tour: {TourName}", Input.TourName);

            return RedirectToPage("/Bookings/MyBookings");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating booking for tour: {TourName}", Input.TourName);
            ErrorMessage = "An error occurred while creating the booking.";
            return Page();
        }
    }
}
