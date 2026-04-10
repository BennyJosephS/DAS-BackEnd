using DoctorAppointmentSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoctorAppointmentSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        private readonly AppDbContext _Db;

        public DoctorController(AppDbContext dbContext)
        {
            _Db = dbContext;
        }

        [Authorize]
        [HttpGet("getDoc")]
        public async Task<IActionResult> getDoc([FromBody] string email)
        {
            var doc = await _Db.Doctors.FirstOrDefaultAsync(u => u.Email == email);

            if (doc == null)
            {
                return BadRequest(new { message = "no doc such that i db" });
            }

            return Ok(doc);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("DespChange")]
        public async Task<IActionResult> DespChange([FromBody] Doctors changes)
        {
            var doctor = await _Db.Doctors.FirstOrDefaultAsync(u => u.Id == changes.Id);

            if (!string.IsNullOrEmpty(changes.Email))
            {
                doctor.Email = changes.Email;
            }

            if (!string.IsNullOrEmpty(changes.Name))
            {
                doctor.Name = changes.Name;
            }

            if (!string.IsNullOrEmpty(changes.Speciality))
            {
                doctor.Speciality = changes.Speciality;
            }

            if (!string.IsNullOrEmpty(changes.Gender))
            {
                doctor.Gender = changes.Gender;
            }

            await _Db.SaveChangesAsync();

            return Ok();
        }

        [Authorize]
        [HttpPost("BookAppointmnet")]
        public async Task<IActionResult> BookAppointment(Appointmentrequest request)
        {

            var doctor = await _Db.Doctors.FirstOrDefaultAsync(u => u.Email == request.DoctorEmail);

            if (string.IsNullOrEmpty(doctor.Name))
            {
                return BadRequest(new { message = "no doctor available as such" });
            }
            var newapt = new Appointment
            {
                Id = 0,
                PatientId = request.PatientId,
                DoctorId = doctor.Id,
                TimeSlot = null,
                Description = request.Description,
                Status = null
            };

            var timeslots = _Db.TimeSlots.AsQueryable();

            var doctime = timeslots.Where(u => u.DoctorId == doctor.Id);

            var lists =await doctime.ToListAsync();

            bool free = true;

            foreach (var n in lists)
            {
                if (free)
                {
                    if (n.EndTime<request.StartTime)
                    {
                        
                    }else if (n.StartTime>request.EndTime)
                    {
                        
                    }
                    else
                    {
                        free = false;
                    }
                }
                else
                {
                    break;
                }
            }

            if (free)
            {
                newapt.Status = "booked";
            }
            else
            {
                newapt.Status = "cancelled";
                await _Db.Appointments.AddAsync(newapt);
                await _Db.SaveChangesAsync();
                return Ok();
            }

            var timeslot = new TimeSlot
            {
                Id = 0,
                Date = DateOnly.FromDateTime(DateTime.UtcNow),
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                Isbooked = true,
                DoctorId = 0
            };

            await _Db.Appointments.AddAsync(newapt);
            await _Db.TimeSlots.AddAsync(timeslot);
            await _Db.SaveChangesAsync();

            return Ok();

        }
    
    }
}
