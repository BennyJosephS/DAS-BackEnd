namespace DoctorAppointmentSystem.Models;

public class Appointmentrequest
{
    public string DoctorEmail;
    public int PatientId;
    public string Description;
    public TimeOnly StartTime;
    public TimeOnly EndTime;
    public DateOnly DateNow;
}