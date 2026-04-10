using DoctorAppointmentSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoctorAppointmentSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrowseDoctor : ControllerBase
    {
        private readonly AppDbContext _Db;
        public BrowseDoctor(AppDbContext options)
        {
            _Db = options;
        }

        [Authorize]
        [HttpGet("ViewDoctors")]
        public async Task<IActionResult> ViewDoctors()
        {
            var doctor = await _Db.Doctors.ToListAsync();
            return Ok(doctor);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("AddDoctor")]
        public async Task<IActionResult> Adddoctors([FromBody] Doctors doc)
        {
            var doctor = new Doctors
            {
                Name = doc.Name,
                Email = doc.Email,
                Speciality = doc.Speciality,
                Gender = doc.Gender
            };

            await _Db.Doctors.AddAsync(doctor);
            await _Db.SaveChangesAsync();

            return Ok(new { message = "add new doctor" });
        }

        // [Authorize]
        // [HttpGet("searchbyname")]
        // public async Task<IActionResult> SearchbyName([FromQuery] string? name, [FromQuery] string? spec)
        // {
        //     
        // }
    }
}
