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
    public class FlightController(IFlight flightRepository, ApplicationDbContext context, IFlightPricingService flightPricing, ISeatService seatService) : ControllerBase
    {
        [HttpPost]
        [Route("add")]
        public async Task<ActionResult> AddFlight([FromBody] AddFlightDto addFlightDto)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest("model mismatch");
            }

            var FlightOwnerId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            var Role = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(FlightOwnerId.ToString()))
            {
                return Unauthorized("FlightOwner claim is missing in the token.");
            }

            if (string.IsNullOrEmpty(Role))
            {
                return Forbid();
            }
            var flightowner = await context.FlightOwners.FirstOrDefaultAsync(fo => fo.UserId == int.Parse(FlightOwnerId));
            if (!flightowner.IsApproved) return BadRequest("Admin approval is required");

            var airline = await context.Airlines.FirstOrDefaultAsync(a => a.AirlineId==addFlightDto.AirlineId);

            if (airline == null) { return BadRequest("Airline not found"); }

            var flight = await flightRepository.AddFlight(addFlightDto, airline.AirlineId);

            if (!flight.flag) { return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Couldn't create the flight" }); }


            return Ok(flight);
        }

        [HttpPut]
        [Route("update/{flightId}")]
        public async Task<ActionResult> UpdateFlight(int flightId, [FromBody] AddFlightDto addFlightDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("model mismatch");
            }

            var FlightOwnerId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            var Role = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(FlightOwnerId.ToString()))
            {
                return Unauthorized("FlightOwner claim is missing in the token.");
            }

            if (string.IsNullOrEmpty(Role))
            {
                return Forbid();
            }
            var flightowner = await context.FlightOwners.FirstOrDefaultAsync(fo => fo.UserId == int.Parse(FlightOwnerId));
            if (!flightowner.IsApproved) return BadRequest("Admin approval is required");

            var airline = await context.Airlines.FirstOrDefaultAsync(a => a.AirlineId == addFlightDto.AirlineId);

            if (airline == null) { return BadRequest("Airline not found"); }

            var flight = await flightRepository.UpdateFlight(addFlightDto, airline.AirlineId,flightId);

            if (!flight.flag) { return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Couldn't update the flight" }); }


            return Ok(flight);
        }



        [HttpPost]
        [Route("addSchedule")]
        public async Task<ActionResult> AddFlightSchedule([FromQuery] int flightId, [FromBody] AddFlightScheduleDto addSchedule)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors)
                                      .Select(e => e.ErrorMessage)
                                      .ToList();
            return BadRequest(new { Errors = errors });

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
            var airline = await context.Airlines.Include(a => a.Flights).Where(a => a.FlightOwnerId == int.Parse(FlightOwnerId)).FirstOrDefaultAsync();
            if (airline != null && airline.Flights.Any())
            {
                // Assuming you want the first flight ID (or modify this as needed)
                if (!airline.Flights.Any(f => f.FlightId == flightId))
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
        [HttpPost]
        [Route("add-pricing/{flightScheduleId}")]
        public async Task<ActionResult> SetFlightPricing(int flightScheduleId, FlightPricingDto flightPricingDto)
        {
            if (!ModelState.IsValid) return BadRequest("Model is invalid");
            var schedule = await context.FlightsSchedules.FirstOrDefaultAsync(fs => fs.FlightScheduleId == flightScheduleId);
            if (schedule == null || schedule.FlightStatus.ToString().ToLower() != "scheduling_process")
            {
                return BadRequest("Invalid flight schedule.");
            }

            var response = await flightPricing.SetFlightPricingAsync(flightPricingDto, schedule);

            if (!response.flag) StatusCode(StatusCodes.Status500InternalServerError, response.message);

            return Ok(response);
        }

        [HttpPost]
        [Route("add-class-pricing/{flightScheduleId}")]
        public async Task<ActionResult> SetFlightScheduleClassPricing(int flightScheduleId, List<ClassPricingDto> classPricingDtos)
        {
            if (!ModelState.IsValid) return BadRequest("Model is invalid");
            var schedule = await context.FlightsSchedules.FindAsync(flightScheduleId);

            if (schedule == null || schedule.FlightStatus.ToString().ToLower() != "scheduling_process")
            {
                return BadRequest("Invalid flight schedule.");
            }

            var response = await seatService.SetClassPricing(classPricingDtos, flightScheduleId);

            if (!response.flag) StatusCode(StatusCodes.Status500InternalServerError, response.message);

            return Ok(response);
        }


        [HttpPost]
        [Route("add-classType-pricing/{flightScheduleId}")]
        public async Task<ActionResult> SetFlightScheduleClassTypePricing(int flightScheduleId, Dictionary<string, List<decimal>> seatTypePricing)
        {
            if (!ModelState.IsValid) return BadRequest("Model is invalid");
            var schedule = await context.FlightsSchedules.FindAsync(flightScheduleId);

            if (schedule == null || schedule.FlightStatus.ToString().ToLower() != "scheduling_process")
            {
                return BadRequest("Invalid flight schedule.");
            }
            var response = await seatService.SetSeatTypePricing(seatTypePricing, flightScheduleId);

            if (!response.flag) StatusCode(StatusCodes.Status500InternalServerError, response.message);

            return Ok(response);
        }

        [HttpPost]
        [Route("add-seat-pricing/{flightScheduleId}")]
        public async Task<ActionResult> SetSeatPricing(int flightScheduleId, SeatDto seatDto)
        {
            if (!ModelState.IsValid) return BadRequest("Model is invalid");
            var schedule = await context.FlightsSchedules.FindAsync(flightScheduleId);

            if (schedule == null || schedule.FlightStatus.ToString().ToLower() != "scheduling_process")
            {
                return BadRequest("Invalid flight schedule.");
            }
            var response = await seatService.AddSeatPricingWithPattern(seatDto, flightScheduleId);

            if (!response.flag) StatusCode(StatusCodes.Status500InternalServerError, response.message);

            return Ok(response);
        }




        [HttpPut]
        [Route("updateStatusReadyToSchedule/{flightScheduleId}")]
        public async Task<ActionResult> UpdateStatusReadyToSchedule(int flightScheduleId)
        {
            var pricingAreFound=await context.SeatTypePricings.FirstOrDefaultAsync(stp=>stp.FlightScheduleId==flightScheduleId);
            if (pricingAreFound == null) return BadRequest("Flight Seat pricing or not set then only we can update the status");

            var cancellationFeeSetupFound=await context.CancellationFees.FirstOrDefaultAsync(cf=>cf.FlightScheduleId==flightScheduleId);
            if (cancellationFeeSetupFound == null) return BadRequest("Cancellation fee setup is needed before schedule the flight");

            var flightSchedule=await context.FlightsSchedules.FirstOrDefaultAsync(fs=>fs.FlightScheduleId== flightScheduleId);

            if (flightSchedule == null) return BadRequest("Flight schedule id not found");

            if(flightSchedule.FlightStatus==FlightStatus.SCHEDULING_PROCESS)
            {
                flightSchedule.FlightStatus = FlightStatus.SCHEDULED;
                context.FlightsSchedules.Update(flightSchedule);
                await context.SaveChangesAsync();
                return Ok("FlightSchedule status is updated to schedule it will be shown for the user");
            }
            return BadRequest("Already Scheduled");
        }

        [HttpGet]
        [Route("flightsByAirline/{id}")]
        public async Task<ActionResult> FlightsByAirlline(int id)
        {
            var flights = await context.Flights.Where(f => f.AirlineId == id).ToListAsync();


            if (flights == null)
            {
                BadRequest("No flights");
            }
            return Ok(flights);
        }

        [HttpGet]
        [Route("flight/{id}")]
        public async Task<ActionResult> GetFlightById(int id)
        {
            var flights = await context.Flights.FirstOrDefaultAsync(f => f.FlightId == id);

            if (flights == null)
            {
                BadRequest("No flights");
            }
            return Ok(flights);
        }
    }
}
