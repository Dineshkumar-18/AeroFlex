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
    public class SeatLayoutController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SeatLayoutController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST api/SeatLayout
        [HttpPost]
        public async Task<IActionResult> CreateSeatLayouts([FromBody] List<SeatLayoutDto> seatLayoutDtos)
        {
            // Check for null or invalid input
            if (seatLayoutDtos == null || seatLayoutDtos.Count == 0)
                return BadRequest("No seat layouts provided.");

            // Ensure that each item in the list is valid
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Map each DTO to a SeatLayout entity
            var seatLayouts = seatLayoutDtos.Select(dto => new SeatLayout
            {
                FlightId = dto.FlightId,
                TotalColumns = dto.TotalColumns,
                LayoutPattern = dto.LayoutPattern,
                SeatTypePattern = dto.SeatTypePattern,
                ClassType = dto.ClassType,
                RowCount = dto.RowCount
            }).ToList();

            // Add all seat layouts to the context
            _context.SeatLayouts.AddRange(seatLayouts);

            // Save changes to the database
            await _context.SaveChangesAsync();

            return Ok(seatLayouts); // You can return the created seat layouts or a success message
        }

        // PUT api/SeatLayout
        [HttpPut]
        public async Task<IActionResult> UpdateSeatLayouts([FromBody] List<SeatLayoutDto> seatLayoutDtos)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Step 1: Retrieve all existing SeatLayouts for the FlightId(s) provided
            var flightId = seatLayoutDtos.FirstOrDefault()?.FlightId;
            if (flightId == null)
            {
                return BadRequest("FlightId is required.");
            }

            var existingSeatLayouts = await _context.SeatLayouts
                .Where(sl => sl.FlightId == flightId)
                .ToListAsync();

            // Step 2: Prepare a list of ClassTypes from the incoming DTO
            var incomingClassTypes = seatLayoutDtos.Select(dto => dto.ClassType).ToList();

            // Step 3: Handle Deletions (if a ClassType is missing in the incoming list, delete it)
            var layoutsToDelete = existingSeatLayouts
                .Where(sl => !incomingClassTypes.Contains(sl.ClassType))
                .ToList();

            if (layoutsToDelete.Count > 0)
            {
                _context.SeatLayouts.RemoveRange(layoutsToDelete);
            }

            // Step 4: Handle Updates and Insertions
            foreach (var seatLayoutDto in seatLayoutDtos)
            {
                var existingLayout = existingSeatLayouts
                    .FirstOrDefault(sl => sl.ClassType == seatLayoutDto.ClassType);

                if (existingLayout != null)
                {
                    // Update common fields if they have changed
                    if (existingLayout.TotalColumns != seatLayoutDto.TotalColumns ||
                        existingLayout.LayoutPattern != seatLayoutDto.LayoutPattern ||
                        existingLayout.SeatTypePattern != seatLayoutDto.SeatTypePattern)
                    {
                        existingLayout.TotalColumns = seatLayoutDto.TotalColumns;
                        existingLayout.LayoutPattern = seatLayoutDto.LayoutPattern;
                        existingLayout.SeatTypePattern = seatLayoutDto.SeatTypePattern;
                        _context.SeatLayouts.Update(existingLayout);
                    }

                    // Handle RowCount updates
                    if (existingLayout.RowCount != seatLayoutDto.RowCount)
                    {
                        existingLayout.RowCount = seatLayoutDto.RowCount;
                        _context.SeatLayouts.Update(existingLayout);
                    }
                }
                else
                {
                    // If this is a new class, add it
                    var newSeatLayout = new SeatLayout
                    {
                        FlightId = seatLayoutDto.FlightId,
                        ClassType = seatLayoutDto.ClassType,
                        TotalColumns = seatLayoutDto.TotalColumns,
                        LayoutPattern = seatLayoutDto.LayoutPattern,
                        SeatTypePattern = seatLayoutDto.SeatTypePattern,
                        RowCount = seatLayoutDto.RowCount
                    };
                    _context.SeatLayouts.Add(newSeatLayout);
                }
            }

            // Step 5: Save all changes
            await _context.SaveChangesAsync();

            return Ok(seatLayoutDtos);
        }



        // GET api/SeatLayout/{flightId}
        [HttpGet("{flightId}")]
        public async Task<IActionResult> GetSeatLayouts(int flightId)
        {
            var seatLayouts = await _context.SeatLayouts
                .Where(sl => sl.FlightId == flightId)   
                .ToListAsync();

            if (seatLayouts == null || seatLayouts.Count == 0)
            {
                return NotFound();
            }

            if (seatLayouts == null || seatLayouts.Count == 0)
            {
                return NotFound();
            }

            // Map the entities to DTOs
            var seatLayoutDtos = seatLayouts.Select(sl => new SeatLayoutDto
            {
                TotalColumns = sl.TotalColumns,
                LayoutPattern = sl.LayoutPattern,
                SeatTypePattern = sl.SeatTypePattern,
                ClassType = sl.ClassType,
                RowCount = sl.RowCount
            }).ToList();


            return Ok(seatLayouts);
        }
    }
}
