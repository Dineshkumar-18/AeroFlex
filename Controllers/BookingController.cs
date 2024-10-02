using AeroFlex.Data;
using AeroFlex.Dtos;
using AeroFlex.Repository.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AeroFlex.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BookingController(ApplicationDbContext context,IBookingService bookingRepository) : ControllerBase
    {
        [HttpPost]
        [Route("createbooking/{flightScheduleId}")]
        public async Task<ActionResult> CreateBooking(int flightScheduleId,BookingDto bookingDto)
        {
            if (!ModelState.IsValid) return BadRequest("Model is invalid");
            var flightSchedule=await context.FlightsSchedules.FirstOrDefaultAsync(fs=>fs.FlightScheduleId== flightScheduleId);
            if (flightSchedule == null) return BadRequest("Particular flight schedule is not found");
            //retrieve user id from the JWT token in the cookie
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            if (userId == null) return Unauthorized();
            var SuceesBooking = await bookingRepository.CreateBookingAsync(bookingDto,int.Parse(userId),flightSchedule.FlightScheduleId);
            if(!SuceesBooking.flag) return BadRequest(SuceesBooking.message);
            return Ok(SuceesBooking);
        }
    }
}
