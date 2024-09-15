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

        [HttpPut]
        [Route("profileCompletion/{flightOwnerId}")]
        public async Task<ActionResult> ProfileCompletion(int flightOwnerId,FlightOwnerProfile profile)
        {
            var flightowner = await _context.FlightOwners.FirstOrDefaultAsync(fo => fo.UserId == flightOwnerId);
            if (flightowner == null) return BadRequest("FlightOwner Not found");

            if(flightowner.IsApproved)
            {
                return BadRequest("Your profile has already approved by admin you can schedule your flights");
            }

            if(!flightowner.IsApproved && !flightowner.IsProfileCompleted)
            {
                flightowner.CompanyName = profile.CompanyName;
                flightowner.CompanyRegistrationNumber= profile.CompanyRegistrationNumber;
                flightowner.CompanyEmail = profile.CompanyEmail;
                flightowner.CompanyPhoneNumber = profile.CompanyPhoneNumber;
                flightowner.OperatingLicenseNumber= profile.OperatingLicenseNumber;
                flightowner.IsProfileCompleted = true;
                _context.FlightOwners.Update(flightowner);
                await _context.SaveChangesAsync();
                return Ok("Profile updated successfully! it is under review of admin");
            }
            return BadRequest("Your profile is waiting for admin approval");

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

    }
}
