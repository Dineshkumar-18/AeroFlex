using AeroFlex.Dtos;
using AeroFlex.Repository.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AeroFlex.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightOwnerAccountController(IFlightOwnerAccount flightOwnerRepository) : ControllerBase
    {
        [HttpPost]
        [Route("register")]
        public async Task<ActionResult> FlightOwnerRegistration(Register register)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Model is invalid");
            }
            var FlightOwnerSuccessRegister = await flightOwnerRepository.CreateAsync(register);
            return Ok(FlightOwnerSuccessRegister);
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult> FlightOwnerLogin(Login login)
        {
            if (!ModelState.IsValid) return BadRequest("Model is invalid");
            var FlightOwnerSuccessLogin = await flightOwnerRepository.SignInAsync(login);
            return Ok(FlightOwnerSuccessLogin);
        }
    }
}
