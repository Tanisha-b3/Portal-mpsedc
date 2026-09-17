using WebApplication1.Models;

namespace WebApplication1.Services;

public interface IAppointmentService
{
    IEnumerable<Appointment> GetAll();
    Appointment? GetById(int id);
    IEnumerable<Appointment> GetByDateRange(DateTime start, DateTime end);
    IEnumerable<Appointment> GetUpcoming();
    IEnumerable<Appointment> GetPast();
    IEnumerable<Appointment> Search(string query);
    Appointment Create(Appointment appointment);
    Appointment? Update(Appointment appointment);
    bool Delete(int id);
    bool ToggleComplete(int id);
    int GetCount();
    int GetCompletedCount();
    int GetUpcomingCount();
}
