namespace DoctorAppointmentSystem.Models;

public class TimeSlot
{
    public int Id { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public bool Isbooked { get; set; }
    public int DoctorId { get; set; }
}