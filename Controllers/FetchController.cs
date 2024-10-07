using AeroFlex.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace AeroFlex.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FetchController(ApplicationDbContext context) : ControllerBase
    {
        [HttpGet]
        [Route("{id}")]

        public async Task<ActionResult> Get(int id)
        {
            var fs = await context.FlightsSchedules.FirstOrDefaultAsync(fs => fs.FlightScheduleId == id);
            if (fs == null) return BadRequest("FlightSchedule Not found");
            return Ok(fs);
        }

        [HttpGet]
        [Route("seats/{id}")]

        public async Task<ActionResult> GetAllSeats(int id)
        {
            var fs = await context.Seats.Where(fs => fs.FlightScheduleId == id).ToListAsync();
            if (fs == null) return BadRequest("Seats Not found");
            return Ok(fs);
        }

        [HttpGet]
        [Route("paymentinfo/{id}")]

        public async Task<ActionResult> Getpaymentinfo(int id)
        {
            var payment = await context.Payments.FirstOrDefaultAsync(p => p.BookingId == id);
            if (payment == null) return BadRequest("payment not found");

            return Ok(payment);

        }
    }
}
