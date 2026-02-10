using TourManagement.Domain.DTOs;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.DTOs;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Web.Pages.Tours;

public class EditModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<EditModel> _logger;

    public EditModel(ITourService tourService, IWebHostEnvironment environment, ILogger<EditModel> logger)
    {
        _tourService = tourService;
        _environment = environment;
        _logger = logger;
    }

    [BindProperty]
    public int Id { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Tour name is required")]
    [StringLength(20, ErrorMessage = "Tour name cannot exceed 20 characters")]
    public string TourName { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Place is required")]
    [StringLength(20, ErrorMessage = "Place cannot exceed 20 characters")]
    public string Place { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Days is required")]
    [Range(1, 99, ErrorMessage = "Days must be between 1 and 99")]
    public int Days { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Price is required")]
    [Range(0, 999999, ErrorMessage = "Price must be a valid amount")]
    public decimal Price { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Locations is required")]
    [StringLength(100, ErrorMessage = "Locations cannot exceed 100 characters")]
    public string Locations { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Tour info is required")]
    [StringLength(250, ErrorMessage = "Tour info cannot exceed 250 characters")]
    public string TourInfo { get; set; } = string.Empty;

    [BindProperty]
    public IFormFile? PictureFile { get; set; }

    [BindProperty]
    public bool IsActive { get; set; }

    public string? ExistingPictureFileName { get; set; }

    public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken)
    {
        try
        {
            var tour = await _tourService.GetByIdAsync(id, cancellationToken);

            if (tour == null)
            {
                return NotFound();
            }

            Id = tour.Id;
            TourName = tour.TourName;
            Place = tour.Place;
            Days = tour.Days;
            Price = tour.Price;
            Locations = tour.Locations;
            TourInfo = tour.TourInfo;
            IsActive = tour.IsActive;
            ExistingPictureFileName = tour.PictureFileName;

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading tour for editing with id {TourId}", id);
            return RedirectToPage("Index");
        }
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var existingTour = await _tourService.GetByIdAsync(Id, cancellationToken);
            if (existingTour == null)
            {
                return NotFound();
            }

            string? pictureFileName = existingTour.PictureFileName;

            if (PictureFile != null && PictureFile.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var extension = Path.GetExtension(PictureFile.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("PictureFile", "Only image files (jpg, jpeg, png, gif) are allowed");
                    ExistingPictureFileName = existingTour.PictureFileName;
                    return Page();
                }

                if (PictureFile.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError("PictureFile", "File size cannot exceed 5MB");
                    ExistingPictureFileName = existingTour.PictureFileName;
                    return Page();
                }

                pictureFileName = $"{Guid.NewGuid()}{extension}";
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var filePath = Path.Combine(uploadsFolder, pictureFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await PictureFile.CopyToAsync(fileStream, cancellationToken);
                }

                if (!string.IsNullOrEmpty(existingTour.PictureFileName))
                {
                    var oldFilePath = Path.Combine(uploadsFolder, existingTour.PictureFileName);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                _logger.LogInformation("Uploaded new file: {FileName}", pictureFileName);
            }

            var dto = new TourUpdateDto
            {
                TourName = TourName,
                Place = Place,
                Days = Days,
                Price = Price,
                Locations = Locations,
                TourInfo = TourInfo,
                PictureFileName = pictureFileName,
                IsActive = IsActive
            };

            await _tourService.UpdateAsync(Id, dto, cancellationToken);

            TempData["SuccessMessage"] = "Tour updated successfully";
            return RedirectToPage("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tour with id {TourId}", Id);
            ModelState.AddModelError(string.Empty, "An error occurred while updating the tour");
            return Page();
        }
    }
}
