﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AeroFlex.Data;
using AeroFlex.Models;
using Microsoft.AspNetCore.Authorization;

namespace AeroFlex.Controllers
{
    [Route("api/[controller]")]
    [ApiController] 
    public class AirportsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AirportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Airports
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Airport>>> GetAirports()
        {
            return await _context.Airports.ToListAsync();
        }

        // GET: api/Airports/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Airport>> GetAirport(int id)
        {
            var airport = await _context.Airports.FindAsync(id);

            if (airport == null)
            {
                return NotFound();
            }

            return airport;
        }

        // PUT: api/Airports/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles ="Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAirport(int id, Airport airport)
        {
            if (id != airport.AirportId)
            {
                return BadRequest();
            }

            _context.Entry(airport).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AirportExists(id))
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

        // POST: api/Airports
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Airport>> PostAirport(Airport airport)
        {
            _context.Airports.Add(airport);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAirport", new { id = airport.AirportId }, airport);
        }

        // DELETE: api/Airports/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAirport(int id)
        {
            var airport = await _context.Airports.FindAsync(id);
            if (airport == null)
            {
                return NotFound();
            }

            _context.Airports.Remove(airport);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("search")]
        public async Task<ActionResult> Search([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Search query cannot be empty.");
            }
            var normalizedQuery = query.ToLower().Replace(" ", "").Replace(".", "").Replace("-", "");

            var results = await _context.Airports
                .Where(x => x.AirportName.ToLower().Replace(" ", "").Replace(".", "").Replace("-", "").Contains(normalizedQuery) ||
                            x.City.ToLower().Replace(" ", "").Replace(".", "").Replace("-", "").Contains(normalizedQuery) ||
                            x.IataCode.ToLower().Replace(" ", "").Replace(".", "").Replace("-", "").Contains(normalizedQuery))
                .ToListAsync();


            return Ok(results);
        }

        private string NormalizeString(string input)
        {
            return new string(input.ToLower()
                .Replace(" ", "")  // Remove spaces
                .Where(c => char.IsLetterOrDigit(c)) // Only keep letters and digits
                .ToArray());
        }



        private bool AirportExists(int id)
        {
            return _context.Airports.Any(e => e.AirportId == id);
        }
    }
}
