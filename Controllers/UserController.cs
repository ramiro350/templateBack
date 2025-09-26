using Microsoft.AspNetCore.Mvc;
using ArqPay.Models;
using ArqPay.Models.Requests;
using ArqPay.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace ArqPay.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _iUserService;

        public UserController(IUserService iUserService)
        {
            _iUserService = iUserService;
        }

        // Get user by email (via query param)
        [HttpGet("by-email")]
        [Authorize]
        public async Task<IActionResult> GetUser([FromQuery] GetUserRequest getUserRequest)
        {
            try
            {
                var user = await _iUserService.GetUser(getUserRequest);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // Get user by id
        [HttpGet("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            try
            {
                var user = await _iUserService.GetUserById(id);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // Get all users
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _iUserService.GetAllUsers();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // Create user
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest user)
        {
            try
            {
                var id = await _iUserService.CreateUser(user);
                return Ok(new { Id = id });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // Update user
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest request)
        {
            try
            {

                _iUserService.UpdateUser(request);

                return Ok("Usuário atualizado");
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // Delete user
        [HttpDelete("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            try
            {
                var rowsAffected = await _iUserService.DeleteUser(id);
                return Ok("Usuário excluído");
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
