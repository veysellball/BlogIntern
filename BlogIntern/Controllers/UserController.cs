using BlogIntern.Data;
using BlogIntern.Dtos;
using BlogIntern.Models;
using BlogIntern.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogIntern.Controllers
{
    [ApiController]
    [Route("api/user")] 
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILoginService _loginService;
        private readonly AppDbContext _context;

        public UserController(IUserService userService, ILoginService loginService, AppDbContext context)
        {
            _userService = userService;
            _loginService = loginService;
            _context = context;
        }

        
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddNewUser([FromBody] UserCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var user = new User
                {
                    Name = dto.Name,
                    Email = dto.Email,
                    Password = dto.Password
                };

                var createdUser = await _userService.AddNewUser(user);
                return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

       
        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var email = User.Identity?.Name;

            if (string.IsNullOrEmpty(email))
                return Unauthorized("Token geçersiz.");

            var user = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
                return NotFound("Kullanıcı bulunamadı.");

            return Ok(new
            {
                user.Id,
                user.Name,
                user.Email,
                Roles = user.UserRoles.Select(r => r.Role.Name)
            });
        }

        
        [Authorize(Roles = "Admin")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsers();
            return Ok(users);
        }

        
        [Authorize(Roles = "Admin")]
        [HttpGet("by-id/{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                var user = await _userService.GetUserById(id);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

      
        [Authorize(Roles = "Admin")]
        [HttpGet("order-by-date")]
        public async Task<IActionResult> GetAllUsersOrderByDate()
        {
            var users = await _userService.GetAllUsersOrderByDate();
            return Ok(users);
        }

     
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteUserById(int id)
        {
            var deleted = await _userService.DeleteUserById(id);
            if (!deleted) return NotFound();
            return NoContent();
        }

       
        [Authorize(Roles = "Admin")]
        [HttpPatch("soft-delete/{id:int}")]
        public async Task<IActionResult> SoftDeleteUserById(int id)
        {
            var updated = await _userService.SoftDeleteUserById(id);
            if (!updated) return NotFound();
            return Ok(new { message = "User deactivated successfully" });
        }

       
        [Authorize(Roles = "Admin")]
        [HttpPatch("reactivate/{id:int}")]
        public async Task<IActionResult> ReActivateUserById(int id)
        {
            var reactivated = await _userService.ReActivateUserById(id);
            if (!reactivated)
                return NotFound(new { message = $"User with id {id} not found." });

            return Ok(new { message = "User reactivated successfully" });
        }
    }
}
