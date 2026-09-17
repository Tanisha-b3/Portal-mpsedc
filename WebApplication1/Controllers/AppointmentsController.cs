using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers;

public class AppointmentsController : Controller
{
    private readonly IAppointmentService _appointmentService;

    public AppointmentsController(IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    // GET: Appointments
    public IActionResult Index(string? search, string? filter)
    {
        IEnumerable<Appointment> appointments;

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(search))
        {
            appointments = _appointmentService.Search(search);
            ViewBag.SearchQuery = search;
        }
        else
        {
            appointments = _appointmentService.GetAll();
        }

        // Apply status filter
        switch (filter?.ToLower())
        {
            case "upcoming":
                appointments = appointments.Where(a => a.IsUpcoming);
                ViewBag.CurrentFilter = "upcoming";
                break;
            case "ongoing":
                appointments = appointments.Where(a => a.IsOngoing);
                ViewBag.CurrentFilter = "ongoing";
                break;
            case "completed":
                appointments = appointments.Where(a => a.IsCompleted);
                ViewBag.CurrentFilter = "completed";
                break;
            case "past":
                appointments = appointments.Where(a => a.IsPast);
                ViewBag.CurrentFilter = "past";
                break;
            default:
                ViewBag.CurrentFilter = "all";
                break;
        }

        // Statistics for dashboard
        ViewBag.TotalCount = _appointmentService.GetCount();
        ViewBag.UpcomingCount = _appointmentService.GetUpcomingCount();
        ViewBag.CompletedCount = _appointmentService.GetCompletedCount();

        return View(appointments.ToList());
    }

    // GET: Appointments/Details/5
    public IActionResult Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var appointment = _appointmentService.GetById(id.Value);
        if (appointment == null)
        {
            return NotFound();
        }

        return View(appointment);
    }

    // GET: Appointments/Create
    public IActionResult Create()
    {
        var model = new CreateAppointmentViewModel
        {
            StartDate = DateTime.Now.AddHours(1),
            EndDate = DateTime.Now.AddHours(2)
        };
        return View(model);
    }

    // POST: Appointments/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(CreateAppointmentViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Validate end date is after start date
            if (model.EndDate <= model.StartDate)
            {
                ModelState.AddModelError("EndDate", "End date must be after start date");
                return View(model);
            }

            var appointment = model.ToAppointment();
            _appointmentService.Create(appointment);
            TempData["SuccessMessage"] = "Appointment created successfully!";
            return RedirectToAction(nameof(Index));
        }
        return View(model);
    }

    // GET: Appointments/Edit/5
    public IActionResult Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var appointment = _appointmentService.GetById(id.Value);
        if (appointment == null)
        {
            return NotFound();
        }

        var model = new EditAppointmentViewModel
        {
            Id = appointment.Id,
            Title = appointment.Title,
            Description = appointment.Description,
            StartDate = appointment.StartDate,
            EndDate = appointment.EndDate,
            Location = appointment.Location,
            Priority = appointment.Priority,
            IsCompleted = appointment.IsCompleted,
            CreatedAt = appointment.CreatedAt
        };

        return View(model);
    }

    // POST: Appointments/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, EditAppointmentViewModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            if (model.EndDate <= model.StartDate)
            {
                ModelState.AddModelError("EndDate", "End date must be after start date");
                return View(model);
            }

            var appointment = new Appointment
            {
                Id = model.Id,
                Title = model.Title,
                Description = model.Description,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                Location = model.Location,
                Priority = model.Priority,
                IsCompleted = model.IsCompleted
            };

            var result = _appointmentService.Update(appointment);
            if (result == null)
            {
                return NotFound();
            }

            TempData["SuccessMessage"] = "Appointment updated successfully!";
            return RedirectToAction(nameof(Index));
        }
        return View(model);
    }

    // GET: Appointments/Delete/5
    public IActionResult Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var appointment = _appointmentService.GetById(id.Value);
        if (appointment == null)
        {
            return NotFound();
        }

        return View(appointment);
    }

    // POST: Appointments/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var result = _appointmentService.Delete(id);
        if (result)
        {
            TempData["SuccessMessage"] = "Appointment deleted successfully!";
        }
        else
        {
            TempData["ErrorMessage"] = "Failed to delete appointment.";
        }
        return RedirectToAction(nameof(Index));
    }

    // POST: Appointments/ToggleComplete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ToggleComplete(int id)
    {
        var result = _appointmentService.ToggleComplete(id);
        if (result)
        {
            TempData["SuccessMessage"] = "Appointment status updated!";
        }
        else
        {
            TempData["ErrorMessage"] = "Failed to update appointment status.";
        }
        return RedirectToAction(nameof(Index));
    }
}
