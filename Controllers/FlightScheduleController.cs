using AeroFlex.Dtos;
using AeroFlex.Models;
using AeroFlex.Repository.Contracts;
using AeroFlex.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        [HttpPost("search-flights")]
        public async Task<IActionResult> SearchFlights(FlightSearchDto flightSearch)
        {

            var outboundFlights = await _flightScheduleService.GetFlightSchedulesAsync(flightSearch.DepartureAirport, flightSearch.ArrivalAirport, flightSearch.DepartureDate,flightSearch.Class,flightSearch.TotalPassengers);

            List<FlightScheduleDTO> returnFlights = null;

            if (flightSearch.ReturnDate.HasValue)
            {
                returnFlights = await _flightScheduleService.GetFlightSchedulesAsync(flightSearch.ArrivalAirport, flightSearch.DepartureAirport, (DateOnly)flightSearch.ReturnDate, flightSearch.Class, flightSearch.TotalPassengers);
            }

            if (!outboundFlights.Any())
            {

                return NotFound("No flight schedules found for the provided criteria.");
            }

            var response = new FlightSearchResponse
            {
                OutboundFlights=outboundFlights,
                ReturnFlights=returnFlights
            };

            return Ok(response);
        }
    }
}
