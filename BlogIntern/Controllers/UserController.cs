using BlogIntern.Data;
using BlogIntern.Dtos;
using BlogIntern.Models;
using BlogIntern.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogIntern.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> AddNewUser([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdUser = await _userService.AddNewUser(user);
                return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsers();
            return Ok(users);
        }

        [HttpGet("{id}")]
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserById(int id)
        {
            var deleted = await _userService.DeleteUserById(id);
            if (!deleted) return NotFound();
            return NoContent();
        }

        [HttpPatch("soft-delete/{id}")]
        public async Task<IActionResult> SoftDeleteUserById(int id)
        {
            var updated = await _userService.SoftDeleteUserById(id);
            if (!updated) return NotFound();
            return Ok(new { message = "User deactivated successfully" });
        }

        [HttpPatch("reactivate/{id}")]
        public async Task<IActionResult> ReActivateUserById(int id)
        {
            var reactivated = await _userService.ReActivateUserById(id);
            if (!reactivated)
                return NotFound(new { message = $"User with id {id} not found." });

            return Ok(new { message = "User reactivated successfully" });
        }

    }
}
