using WebApplication1.Models;

namespace WebApplication1.Services;

public class InMemoryAppointmentService : IAppointmentService
{
    private readonly List<Appointment> _appointments = new();
    private int _nextId = 1;

    public InMemoryAppointmentService()
    {
        // Seed with sample data
        SeedData();
    }

    public IEnumerable<Appointment> GetAll()
    {
        return _appointments.OrderByDescending(a => a.StartDate);
    }

    public Appointment? GetById(int id)
    {
        return _appointments.FirstOrDefault(a => a.Id == id);
    }

    public IEnumerable<Appointment> GetByDateRange(DateTime start, DateTime end)
    {
        return _appointments
            .Where(a => a.StartDate >= start && a.StartDate <= end)
            .OrderBy(a => a.StartDate);
    }

    public IEnumerable<Appointment> GetUpcoming()
    {
        return _appointments
            .Where(a => a.StartDate > DateTime.Now && !a.IsCompleted)
            .OrderBy(a => a.StartDate);
    }

    public IEnumerable<Appointment> GetPast()
    {
        return _appointments
            .Where(a => a.EndDate < DateTime.Now || a.IsCompleted)
            .OrderByDescending(a => a.StartDate);
    }

    public IEnumerable<Appointment> Search(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return GetAll();

        var lowerQuery = query.ToLower();
        return _appointments
            .Where(a => a.Title.ToLower().Contains(lowerQuery) ||
                       (a.Description != null && a.Description.ToLower().Contains(lowerQuery)) ||
                       (a.Location != null && a.Location.ToLower().Contains(lowerQuery)))
            .OrderBy(a => a.StartDate);
    }

    public Appointment Create(Appointment appointment)
    {
        appointment.Id = _nextId++;
        appointment.CreatedAt = DateTime.Now;
        _appointments.Add(appointment);
        return appointment;
    }

    public Appointment? Update(Appointment appointment)
    {
        var existing = GetById(appointment.Id);
        if (existing == null) return null;

        existing.Title = appointment.Title;
        existing.Description = appointment.Description;
        existing.StartDate = appointment.StartDate;
        existing.EndDate = appointment.EndDate;
        existing.Location = appointment.Location;
        existing.Priority = appointment.Priority;
        existing.IsCompleted = appointment.IsCompleted;
        existing.UpdatedAt = DateTime.Now;

        return existing;
    }

    public bool Delete(int id)
    {
        var appointment = GetById(id);
        if (appointment == null) return false;

        _appointments.Remove(appointment);
        return true;
    }

    public bool ToggleComplete(int id)
    {
        var appointment = GetById(id);
        if (appointment == null) return false;

        appointment.IsCompleted = !appointment.IsCompleted;
        appointment.UpdatedAt = DateTime.Now;
        return true;
    }

    public int GetCount() => _appointments.Count;
    public int GetCompletedCount() => _appointments.Count(a => a.IsCompleted);
    public int GetUpcomingCount() => _appointments.Count(a => a.StartDate > DateTime.Now && !a.IsCompleted);

    private void SeedData()
    {
        _appointments.AddRange(new List<Appointment>
        {
            new Appointment
            {
                Id = _nextId++,
                Title = "Team Standup Meeting",
                Description = "Daily team synchronization meeting to discuss progress and blockers.",
                StartDate = DateTime.Now.AddDays(1).Date.AddHours(9),
                EndDate = DateTime.Now.AddDays(1).Date.AddHours(9).AddMinutes(30),
                Location = "Conference Room A",
                Priority = AppointmentPriority.Medium,
                IsCompleted = false,
                CreatedAt = DateTime.Now.AddDays(-2)
            },
            new Appointment
            {
                Id = _nextId++,
                Title = "Project Review with Client",
                Description = "Quarterly project review presentation with the client stakeholders.",
                StartDate = DateTime.Now.AddDays(3).Date.AddHours(14),
                EndDate = DateTime.Now.AddDays(3).Date.AddHours(16),
                Location = "Virtual - Zoom",
                Priority = AppointmentPriority.High,
                IsCompleted = false,
                CreatedAt = DateTime.Now.AddDays(-5)
            },
            new Appointment
            {
                Id = _nextId++,
                Title = "Lunch with Sarah",
                Description = "Catching up over lunch at the new Italian place.",
                StartDate = DateTime.Now.AddDays(2).Date.AddHours(12),
                EndDate = DateTime.Now.AddDays(2).Date.AddHours(13),
                Location = "Bella Italia Restaurant",
                Priority = AppointmentPriority.Low,
                IsCompleted = false,
                CreatedAt = DateTime.Now.AddDays(-1)
            },
            new Appointment
            {
                Id = _nextId++,
                Title = "Dentist Appointment",
                Description = "Regular checkup and cleaning.",
                StartDate = DateTime.Now.AddDays(5).Date.AddHours(10),
                EndDate = DateTime.Now.AddDays(5).Date.AddHours(11),
                Location = "Downtown Dental Clinic",
                Priority = AppointmentPriority.Medium,
                IsCompleted = false,
                CreatedAt = DateTime.Now.AddDays(-7)
            },
            new Appointment
            {
                Id = _nextId++,
                Title = "Code Review Session",
                Description = "Review pull requests for the new feature branch.",
                StartDate = DateTime.Now.AddDays(1).Date.AddHours(15),
                EndDate = DateTime.Now.AddDays(1).Date.AddHours(16),
                Location = "Virtual - Teams",
                Priority = AppointmentPriority.Medium,
                IsCompleted = false,
                CreatedAt = DateTime.Now.AddDays(-1)
            },
            new Appointment
            {
                Id = _nextId++,
                Title = "Gym Workout",
                Description = "Weekly strength training session.",
                StartDate = DateTime.Now.AddDays(4).Date.AddHours(7),
                EndDate = DateTime.Now.AddDays(4).Date.AddHours(8),
                Location = "FitLife Gym",
                Priority = AppointmentPriority.Low,
                IsCompleted = false,
                CreatedAt = DateTime.Now.AddDays(-3)
            }
        });
    }
}
