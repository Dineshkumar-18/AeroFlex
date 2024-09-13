using AeroFlex.Repository.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AeroFlex.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FlightScheduleController : ControllerBase
    {
        private readonly IFlightScheduleRepository _flightScheduleService;

        public FlightScheduleController(IFlightScheduleRepository flightScheduleService)
        {
            _flightScheduleService = flightScheduleService;
        }

        [HttpGet("flightScheduleSearch")]
        public async Task<IActionResult> GetFlightSchedules(string departureAirport, string arrivalAirport, DateTime date)
        {
            var flightSchedules = await _flightScheduleService.GetFlightSchedulesAsync(departureAirport, arrivalAirport, date);

            if (flightSchedules == null || !flightSchedules.Any())
            {
                return NotFound("No flight schedules found for the provided criteria.");
            }

            return Ok(flightSchedules);
        }
    }
}
