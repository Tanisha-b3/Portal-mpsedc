using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models;

public class CreateAppointmentViewModel
{
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

    [StringLength(200, ErrorMessage = "Location cannot exceed 200 characters")]
    public string? Location { get; set; }

    [Display(Name = "Priority")]
    public AppointmentPriority Priority { get; set; } = AppointmentPriority.Medium;

    public Appointment ToAppointment()
    {
        return new Appointment
        {
            Title = Title,
            Description = Description,
            StartDate = StartDate,
            EndDate = EndDate,
            Location = Location,
            Priority = Priority,
            IsCompleted = false,
            CreatedAt = DateTime.Now
        };
    }
}
