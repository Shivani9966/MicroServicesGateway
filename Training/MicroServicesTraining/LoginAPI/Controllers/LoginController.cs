using LoginAPI.Models;
using LoginAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace LoginAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ILoginService _loginService;
        public LoginController(ILoginService loginService)
        {
            _loginService = loginService;
        }

        [HttpPost, Route("Authenticate")]
        public IActionResult Login(LoginModel user)
        {
            LoginModel loginResponse = _loginService.AuthenticateUser(user);
            if (loginResponse == null)
                return BadRequest(new { message = "Invalid username or password! Try again." } );
            return Ok(loginResponse);
        }
    }
}