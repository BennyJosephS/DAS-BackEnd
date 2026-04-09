namespace DoctorAppointmentSystem.Models;

public class LoginResponse
{
    public string Token { get; set; }
    public DateTime RefreshExpiry { get; set; }
    public string RefreshToken { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
}