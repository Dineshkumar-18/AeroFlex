using AeroFlex.Data;
using AeroFlex.Dtos;
using AeroFlex.Models;
using AeroFlex.Repository.Contracts;
using AeroFlex.Repository.Implementations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AeroFlex.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController(IAdminAccount adminRepository,ApplicationDbContext context) : ControllerBase
    {
        [HttpPost]
        [Route("register")]
        public async Task<ActionResult> AdminRegistration(Register register)
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
        public async Task<ActionResult> AdminLogin(Login login)
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

        [HttpPut]
        [Authorize(Roles="Admin")]
        [Route("FlightOwnerReview/{flightOwnerId}")]
        public async Task<ActionResult> FlightOwnerReview(int flightOwnerId)
        {
            var flightowner = await context.FlightOwners.FirstOrDefaultAsync(fo => fo.UserId == flightOwnerId);
            if (flightowner == null) return BadRequest("FlightOwner Not found");

            if(!flightowner.IsApproved && flightowner.IsProfileCompleted)
            {
                flightowner.IsApproved = true;
                flightowner.ApprovalStatus=ApprovalStatus.Approved;
                context.FlightOwners.Update(flightowner);
                await context.SaveChangesAsync();
                return Ok("Updated successfully");
            }
            else if(!flightowner.IsProfileCompleted)
            {
                return BadRequest("You need to submit the company details to get approval from the admin");
            }
            return BadRequest("Already approved");
        }

    }
}
