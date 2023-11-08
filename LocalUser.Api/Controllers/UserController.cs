using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Users.Domain.Entity;
using Users.Domain.Interface;

namespace Users.Api.Controllers
{
    [Route("api/UsersAuth")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepo;

        public UserController(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            var loginResponse = await _userRepo.Login(model);
            if (loginResponse.User == null || string.IsNullOrEmpty(loginResponse.Token))
            {
                return BadRequest(new { message = "Username or password is incorrect" });
            }
            return Ok(loginResponse);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequest model)
        {
            if (!_userRepo.IsUniqueUser(model.UserName))
            {
                return BadRequest("Username already exists");
            }

            var user = await _userRepo.Register(model);
            if (user == null)
            {
                return BadRequest("Error while registering");
            }

            return Ok(new { message = "User registered successfully" });
        }
    }
}
