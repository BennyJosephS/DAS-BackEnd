using System.ComponentModel.DataAnnotations;

namespace DoctorAppointmentSystem.Models;

public class RegisterRequest
{
    [Required(ErrorMessage = "email is required")]
    [EmailAddress(ErrorMessage = "this is not email format")]
    public string Email { get; set; }
    [Required(ErrorMessage = "Password is required")]
    [MinLength(6,ErrorMessage = "passwrod is not enough length")]
    public string Password { get; set; }
    [Required]
    public string Role { get; set; } = "User";
    [Required]
    public string Gender { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public int Age { get; set; }
    [Required]
    [Phone(ErrorMessage = "enter a valid phone number")]
    public string PhonenNumber { get; set; }
}