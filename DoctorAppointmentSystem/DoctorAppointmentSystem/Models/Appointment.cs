namespace DoctorAppointmentSystem.Models;

public class Appointment
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public TimeSlot TimeSlot { get; set; }
    public string Description { get; set; }
    public string Status { get; set; }
}