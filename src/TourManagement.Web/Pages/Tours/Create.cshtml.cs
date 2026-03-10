using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Web.Pages.Tours;

public class ToursCreateModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<ToursCreateModel> _logger;

    public ToursCreateModel(ITourService tourService, IWebHostEnvironment environment, ILogger<ToursCreateModel> logger)
    {
        _tourService = tourService;
        _environment = environment;
        _logger = logger;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new InputModel();

    public string? ErrorMessage { get; set; }

    public class InputModel
    {
        [Required]
        [StringLength(20)]
        public string TourName { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Place { get; set; } = string.Empty;

        [Required]
        [Range(1, 365)]
        public int Days { get; set; }

        [Required]
        [Range(0, 999999)]
        public decimal Price { get; set; }

        [Required]
        [StringLength(100)]
        public string Locations { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string TourInfo { get; set; } = string.Empty;

        public IFormFile? PictureFile { get; set; }
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            string? picturePath = null;

            if (Input.PictureFile != null)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(Input.PictureFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await Input.PictureFile.CopyToAsync(fileStream);
                }

                picturePath = fileName;
            }

            var tour = new Tour
            {
                TourName = Input.TourName,
                Place = Input.Place,
                Days = Input.Days,
                Price = Input.Price,
                Locations = Input.Locations,
                TourInfo = Input.TourInfo,
                PicturePath = picturePath
            };

            await _tourService.CreateTourAsync(tour);

            _logger.LogInformation("Tour created successfully: {TourName}", Input.TourName);

            return RedirectToPage("/Tours/Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating tour: {TourName}", Input.TourName);
            ErrorMessage = "An error occurred while creating the tour.";
            return Page();
        }
    }
}
