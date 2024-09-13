using AeroFlex.Dtos;
using AeroFlex.Models;
using AeroFlex.Repository.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AeroFlex.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AirlinesController : ControllerBase
    {
        private readonly IAirlineRepository _airlineRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AirlinesController(IAirlineRepository airlineRepository)
        {
            _airlineRepository = airlineRepository;
        }

        // GET: api/Airlines
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<AirlineDto>>> GetAirlines()
        {
            var airlines = await _airlineRepository.GetAllAirlinesAsync();
            return Ok(airlines);
        }

        [HttpGet]
        [Authorize(Roles = "FlightOwner")]
        [Route("airlinesByflightowner")]
        public async Task<ActionResult<IEnumerable<AirlineDto>>> GetAirlinesByFlightowner()
        {
            int? flightOwnerId = GetFlightOwnerIdFromToken();
            if (flightOwnerId == null)
            {
                return Unauthorized();
            }

            var airlines = await _airlineRepository.GetAllAirlinesByFlightownerAsync(flightOwnerId);
            return Ok(airlines);
        }




        // GET: api/Airlines/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<AirlineDto>> GetAirline(int id)
        {
            var airline = await _airlineRepository.GetAirlineByIdAsync(id);

            if (airline == null)
            {
                return NotFound();
            }

            return Ok(airline);
        }

        // POST: api/Airlines
        [HttpPost]
        [Authorize(Roles = "FlightOwner")]
        public async Task<ActionResult<AirlineDto>> PostAirline(AirlineDto airlineDto)
        {
            var flightOwnerId = GetFlightOwnerIdFromToken();
            if (flightOwnerId == null)
            {
                return Unauthorized();
            }

            var airline = new Airline
            {
                AirlineName = airlineDto.AirlineName,
                IataCode = airlineDto.IataCode,
                Headquarters = airlineDto.Headquarters,
                Country = airlineDto.Country,
                FlightOwnerId = flightOwnerId.Value
            };

            var createdAirline = await _airlineRepository.CreateAirlineAsync(airline);
            return CreatedAtAction(nameof(GetAirline), new { id = createdAirline.AirlineName }, createdAirline);
        }

        [Authorize(Roles = "FlightOwner")]
        // PUT: api/Airlines/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAirline(int id, AirlineDto airlineDto)
        {
            var existingAirline = await _airlineRepository.GetAirlineByIdAsync(id);
            if (existingAirline == null)
            {
                return NotFound();
            }

            var airlineToUpdate = new Airline
            {
                AirlineId = id,
                AirlineName = airlineDto.AirlineName,
                IataCode = airlineDto.IataCode,
                Headquarters = airlineDto.Headquarters,
                Country = airlineDto.Country,
                FlightOwnerId = GetFlightOwnerIdFromToken().Value
            };

            var updatedAirline = await _airlineRepository.UpdateAirlineAsync(airlineToUpdate);
            return NoContent();
        }

        // DELETE: api/Airlines/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAirline(int id)
        {
            var deleted = await _airlineRepository.DeleteAirlineAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }

        // Method to extract FlightOwnerId from JWT token
        private int? GetFlightOwnerIdFromToken()
        {
            var flightOwnerId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            var Role = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;


            if (string.IsNullOrEmpty(Role))
            {
                return null;
            }


            return int.TryParse(flightOwnerId, out var result) ? result : null;
        }
    }

}
