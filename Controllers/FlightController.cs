using AeroFlex.Data;
using AeroFlex.Dtos;
using AeroFlex.Models;
using AeroFlex.Repository.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Security.Claims;

namespace AeroFlex.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "FlightOwner")]
    [ApiController]
    public class FlightController(IFlight flightRepository,ApplicationDbContext context) : ControllerBase
    {
        [HttpPost]
        [Route("add")]
        public async Task<ActionResult> AddFlight(AddFlightDto addFlightDto)
        {
            if(!ModelState.IsValid) return BadRequest("Model is invalid");

            var FlightOwnerId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            var Role = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;


            if (string.IsNullOrEmpty(FlightOwnerId.ToString()))
            {
                return Unauthorized("FlightOwner claim is missing in the token.");
            }

            if (string.IsNullOrEmpty(Role))
            {
                return Forbid("You are not authorized to perform this action.");
            }
            var airline = await context.Airlines.FirstOrDefaultAsync(a => a.FlightOwnerId == int.Parse(FlightOwnerId));

            if (airline == null) { return BadRequest("Airline not found"); }

            var flight = await flightRepository.AddFlight(addFlightDto, airline.AirlineId);

            if (!flight.flag) { return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Couldn't create the flight" }); }

            return Ok(flight);
        }


        [HttpPost]
        [Route("addSchedule")]
        public async Task<ActionResult> AddFlightSchedule([FromQuery] int flightId, [FromBody] FlightScheduleDTO addSchedule)
        {
            if (!ModelState.IsValid) return BadRequest("Model is invalid");

            var FlightOwnerId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            var Role = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;


            if (string.IsNullOrEmpty(FlightOwnerId.ToString()))
            {
                return Unauthorized("FlightOwner claim is missing in the token.");
            }

            if (string.IsNullOrEmpty(Role))
            {
                return Forbid("You are not authorized to perform this action.");
            }
            var airline = await context.Airlines.Include(a=>a.Flights).Where(a => a.FlightOwnerId == int.Parse(FlightOwnerId)).FirstOrDefaultAsync();
            if (airline != null && airline.Flights.Any())
            {
                // Assuming you want the first flight ID (or modify this as needed)
                if(!airline.Flights.Any(f=>f.FlightId==flightId))
                {
                   return Unauthorized("You are not allowed schedule the flight");
                }

                var fli = await flightRepository.AddFlightSchedule(addSchedule, flightId);

                if (!fli.flag) { return StatusCode(StatusCodes.Status400BadRequest, fli.message); }

                return Ok(fli);
            }
            else
            {
                return Forbid();
            }
        }


    }
}
