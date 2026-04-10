
using DoctorAppointmentSystem.Models;
using DoctorAppointmentSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;

namespace DoctorAppointmentSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly TokenServices _tokenServices;
        private readonly AppDbContext _Db;
        public AuthController(TokenServices token, AppDbContext App)
        {
            _tokenServices = token;
            _Db = App;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var userifexsist =await _Db.Users.AnyAsync(u => u.Email == request.Email);

            if (userifexsist)
            {
                return BadRequest(new { message = "user already exsist" });
            }

            var passwordhash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                Age = request.Age,
                Email = request.Email,
                Gender = request.Gender,
                Name = request.Name,
                PasswordHash = passwordhash,
                PhoneNumber = request.PhonenNumber,
                Role = "User"
            };

            await _Db.Users.AddAsync(user);
            await _Db.SaveChangesAsync();
            return Ok(new { message = "registered" });
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _Db.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
            {
                return BadRequest(new { message = "Wrong Credentials" });
            }

            var passwordiscorrect = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

            if (!passwordiscorrect)
            {
                return BadRequest(new { message = "enter the right credentials" });
            }

            var generatetoken = _tokenServices.GenerateToken(request.Email, user.Role);

            var refreshtoken = _tokenServices.GenerateRefresh();

            var refreshexpriy = _tokenServices.GetRefreshTokenExpiry();

            user.RefreshToken = refreshtoken;
            user.RefreshTokenExpiryAt = refreshexpriy;
            await _Db.SaveChangesAsync();

            return Ok(new LoginResponse
            {
                Token = generatetoken,
                RefreshExpiry = refreshexpriy,
                RefreshToken = refreshtoken,
                Email = request.Email,
                Role = user.Role
            });
        }

        [Authorize]
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefershRequest request)
        {
            var userdetail = await _Db.Users.FirstOrDefaultAsync(u =>
                u.RefreshToken == request.RefreshToken);

            if (userdetail == null)
            {
                return Unauthorized(new { message = "sorry invalid token" });
            }

            if (userdetail.RefreshTokenExpiryAt < DateTime.UtcNow)
            {
                return Unauthorized(new { message = "token time expired" });
            }

            var newtoken = _tokenServices.GenerateToken(userdetail.Email, userdetail.Role);

            var refresh = _tokenServices.GenerateRefresh();
            var refreshtime = _tokenServices.GetRefreshTokenExpiry();

            userdetail.RefreshToken = refresh;
            userdetail.RefreshTokenExpiryAt = refreshtime;
            await _Db.SaveChangesAsync();

            return Ok(new LoginResponse
            {
                Token = newtoken,
                Email = userdetail.Email,
                RefreshExpiry = DateTime.UtcNow.AddMinutes(60),
                RefreshToken = refresh,
                Role = userdetail.Role
            });
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> logout([FromBody] RefershRequest request)
        {
            var userdetail = await _Db.Users.FirstOrDefaultAsync(u =>
        
                u.RefreshToken == request.RefreshToken
            );

            if (userdetail == null)
            {
                return Unauthorized(new { message = "wrong token" });
            }

            userdetail.RefreshToken = null;
            userdetail.RefreshTokenExpiryAt = null;

            await _Db.SaveChangesAsync();

            return Ok(new { message = "logout success" });
        }
    }
}
