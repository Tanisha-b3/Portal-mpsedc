using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models;

public class Appointment
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Title is required")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 100 characters")]
    public string Title { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Start date is required")]
    [Display(Name = "Start Date")]
    public DateTime StartDate { get; set; } = DateTime.Now;

    [Required(ErrorMessage = "End date is required")]
    [Display(Name = "End Date")]
    public DateTime EndDate { get; set; } = DateTime.Now.AddHours(1);

    [Display(Name = "Location")]
    [StringLength(200, ErrorMessage = "Location cannot exceed 200 characters")]
    public string? Location { get; set; }

    [Display(Name = "Is Completed")]
    public bool IsCompleted { get; set; }

    [Display(Name = "Priority")]
    public AppointmentPriority Priority { get; set; } = AppointmentPriority.Medium;

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }

    // Computed property to check if appointment is upcoming
    public bool IsUpcoming => StartDate > DateTime.Now && !IsCompleted;

    // Computed property to check if appointment is ongoing
    public bool IsOngoing => StartDate <= DateTime.Now && EndDate >= DateTime.Now && !IsCompleted;

    // Computed property to check if appointment is past
    public bool IsPast => EndDate < DateTime.Now || IsCompleted;
}

public enum AppointmentPriority
{
    Low,
    Medium,
    High,
    Urgent
}
