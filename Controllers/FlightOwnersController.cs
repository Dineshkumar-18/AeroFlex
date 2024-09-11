using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AeroFlex.Data;
using AeroFlex.Models;
using AeroFlex.Dtos;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace AeroFlex.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles="FlightOwner")]
    [ApiController]
    public class FlightOwnersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FlightOwnersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/FlightOwners
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FlightOwner>>> GetFlightOwners()
        {
            return await _context.FlightOwners.ToListAsync();
        }

        // GET: api/FlightOwners/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FlightOwner>> GetFlightOwner(int id)
        {
            var flightOwner = await _context.FlightOwners.FindAsync(id);

            if (flightOwner == null)
            {
                return NotFound();
            }

            return flightOwner;
        }

        // PUT: api/FlightOwners/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFlightOwner(int id, FlightOwner flightOwner)
        {
            if (id != flightOwner.UserId)
            {
                return BadRequest();
            }

            _context.Entry(flightOwner).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FlightOwnerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/FlightOwners
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<FlightOwner>> PostFlightOwner(FlightOwner flightOwner)
        {
            _context.FlightOwners.Add(flightOwner);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFlightOwner", new { id = flightOwner.UserId }, flightOwner);
        }

        // DELETE: api/FlightOwners/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFlightOwner(int id)
        {
            var flightOwner = await _context.FlightOwners.FindAsync(id);
            if (flightOwner == null)
            {
                return NotFound();
            }

            _context.FlightOwners.Remove(flightOwner);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FlightOwnerExists(int id)
        {
            return _context.FlightOwners.Any(e => e.UserId == id);
        }


        [HttpGet]
        [Route("getallflightinfo/{id}")]
        public async Task<ActionResult> FlightInfo(int id)
        {
            var flights = await (from airline in _context.Airlines
                           join flight in _context.Flights
                           on airline.AirlineId equals flight.AirlineId
                           where airline.FlightOwnerId == id
                           select new
                           {
                              flight.FlightId,
                              flight.FlightNumber,
                              airline.AirlineName,
                              FlightType = flight.FlightType.ToString(),
                              flight.TotalSeats,
                              DepartureAirport=flight.DepartureAirport.AirportName,
                              ArrivalAirport=flight.ArrivalAirport.AirportName
                           }).ToListAsync();



            if (flights.Count == 0)
            {
                return NotFound();
            }
            return Ok(flights);    
        }

        //[HttpPost]
        //[Route("addSchedule")]
        //public async Task<ActionResult> AddFlightSchedule(FlightScheduleDTO flightScheduleDTO)
        //{
        //    var FlightOwnerId = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        //    var Role = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;

        //    if(string.IsNullOrEmpty(FlightOwnerId.ToString()))
        //    {
        //        return Unauthorized("FlightOwner claim is missing in the token.");
        //    }

        //    if (string.IsNullOrEmpty(Role))
        //    {
        //        return Forbid("You are not authorized to perform this action.");
        //    }

        //    var flight = await _context.Flights.Include(f => f.Airline).Where(f => f.FlightNumber == flightScheduleDTO.FlightNumber && f.Airline.FlightOwnerId == FlightOwnerId).ToListAsync();
        //    if (flight == null)
        //    {
        //        return
        //    }

        //    //Getting departureAirport id
        //    var departureAirportId = await _context.Airports.Where(a => a.AirportName == flightScheduleDTO.DepartureAirport).FirstOrDefaultAsync();
        //    if (departureAirportId == null)
        //    {

        //    }
        //    //Getting arrivalAirport id
        //    var arrivalAirportId = await _context.Airports.Where(a => a.AirportName == flightScheduleDTO.ArrivalAirport).FirstOrDefaultAsync();
        //}
    }
}
