using AeroFlex.Dtos;
using AeroFlex.Repository.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AeroFlex.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController(IAdminAccount adminRepository) : ControllerBase
    {
        [HttpPost]
        [Route("register")]
        public async Task<ActionResult> FlightOwnerRegistration(Register register)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Model is invalid");
            }
            var AdminSuccessRegister = await adminRepository.CreateAsync(register);
            return Ok(AdminSuccessRegister);
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult> FlightOwnerLogin(Login login)
        {
            if (!ModelState.IsValid) return BadRequest("Model is invalid");
            var AdminSuccessLogin = await adminRepository.SignInAsync(login);
            return Ok(AdminSuccessLogin);
        }

        [HttpGet("view-cookies")]
        public IActionResult ViewCookies()
        {
            // Get a specific cookie
            if (Request.Cookies.TryGetValue("AuthToken", out var authToken))
            {
                return Ok(new { message = "Cookie Found", token = authToken });
            }

            // No cookie found
            return NotFound(new { message = "Cookie not found" });
        }
    }
}
