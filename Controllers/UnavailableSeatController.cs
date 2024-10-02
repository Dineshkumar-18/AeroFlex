using Microsoft.AspNetCore.Mvc;
using AeroFlex.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AeroFlex.Data;
using AeroFlex.Dtos;

namespace AeroFlex.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnavailableSeatController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UnavailableSeatController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST api/UnavailableSeat (Add unavailable seats)
        [HttpPost]
        public async Task<IActionResult> AddUnavailableSeats([FromBody] UnavailableSeatsRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Create a list to hold the UnavailableSeats entities
            var unavailableSeats = new List<UnavailableSeats>();

            // Populate the list with the seat numbers and their corresponding class types from the request
            foreach (var entry in request.Seats)
            {
                var classType = entry.Key; // The class type (e.g., "Economy")
                var seatNumbers = entry.Value; // The list of seat numbers

                foreach (var seatNumber in seatNumbers)
                {
                    unavailableSeats.Add(new UnavailableSeats
                    {
                        FlightId = request.FlightId,
                        SeatNumber = seatNumber,
                        ClassType = classType // Set the class type
                    });
                }
            }

            _context.UnavailableSeats.AddRange(unavailableSeats);
            await _context.SaveChangesAsync();

            return Ok(unavailableSeats);
        }

        // DELETE api/UnavailableSeat/{flightId} (Remove multiple unavailable seats)
        [HttpDelete("{flightId}")]
        public async Task<IActionResult> RemoveUnavailableSeats(int flightId, [FromBody] List<string> seatNumbers)
        {
            if (seatNumbers == null || seatNumbers.Count == 0)
            {
                return BadRequest("Seat numbers cannot be empty.");
            }

            var seatsToRemove = await _context.UnavailableSeats
                .Where(us => us.FlightId == flightId && seatNumbers.Contains(us.SeatNumber))
                .ToListAsync();

            if (seatsToRemove.Count == 0)
            {
                return NotFound("No unavailable seats found for the specified flight ID.");
            }

            _context.UnavailableSeats.RemoveRange(seatsToRemove);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET api/UnavailableSeat/{flightId} (Retrieve unavailable seats for a flight)
        [HttpGet("{flightId}")]
        public async Task<IActionResult> GetUnavailableSeats(int flightId)
        {
            var unavailableSeats = await _context.UnavailableSeats
                .Where(us => us.FlightId == flightId)
                .ToListAsync();

            if (unavailableSeats == null || unavailableSeats.Count == 0)
            {
                return NotFound();
            }

            return Ok(unavailableSeats);
        }
    }
}
