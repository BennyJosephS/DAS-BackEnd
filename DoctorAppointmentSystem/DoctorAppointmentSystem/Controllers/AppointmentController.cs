using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoctorAppointmentSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly AppDbContext _Db;
        public AppointmentController(AppDbContext options)
        {
            _Db = options;
        }

        [Authorize]
        [HttpGet("AllApt")]
        public async Task<IActionResult> AllApt([FromBody] int patientId)
        {
            var listss = _Db.Appointments.AsQueryable();

            var apt = listss.Where(u => u.PatientId == patientId);

            var result=await apt.ToListAsync();

            return Ok(result);
        }
    }
}
