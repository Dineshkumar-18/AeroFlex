using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AeroFlex.Data;
using AeroFlex.Models;

namespace AeroFlex.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaggagesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BaggagesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Baggages
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Baggage>>> GetBaggages()
        {
            return await _context.Baggages.ToListAsync();
        }

        // GET: api/Baggages/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Baggage>> GetBaggage(int id)
        {
            var baggage = await _context.Baggages.FindAsync(id);

            if (baggage == null)
            {
                return NotFound();
            }

            return baggage;
        }

        // PUT: api/Baggages/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBaggage(int id, Baggage baggage)
        {
            if (id != baggage.BaggageId)
            {
                return BadRequest();
            }

            _context.Entry(baggage).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BaggageExists(id))
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

        // POST: api/Baggages
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Baggage>> PostBaggage(Baggage baggage)
        {
            _context.Baggages.Add(baggage);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBaggage", new { id = baggage.BaggageId }, baggage);
        }

        // DELETE: api/Baggages/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBaggage(int id)
        {
            var baggage = await _context.Baggages.FindAsync(id);
            if (baggage == null)
            {
                return NotFound();
            }

            _context.Baggages.Remove(baggage);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BaggageExists(int id)
        {
            return _context.Baggages.Any(e => e.BaggageId == id);
        }


        //New business logics for Baggage 

        // GET: api/Baggages/flight/{flightId}
        [HttpGet("flight/{flightId}")]
        public async Task<ActionResult<IEnumerable<Baggage>>> GetBaggagesByFlight(int flightId)
        {
            return await _context.Baggages
                .Where(b => b.FlightId == flightId)
                .ToListAsync();
        }

        // GET: api/Baggages/passenger/{passengerId}
        [HttpGet("passenger/{passengerId}")]
        public async Task<ActionResult<IEnumerable<Baggage>>> GetBaggagesByPassenger(int passengerId)
        {
            return await _context.Baggages
                .Where(b => b.PassengerId == passengerId)
                .ToListAsync();
        }

        // Calculate Total Check-in Weight by Flight

        // GET: api/Baggages/flight/{flightId}/checkinweight
        [HttpGet("flight/{flightId}/checkinweight")]
        public async Task<ActionResult<decimal>> GetTotalCheckInWeightByFlight(int flightId)
        {
            var totalWeight = await _context.Baggages
                .Where(b => b.FlightId == flightId)
                .SumAsync(b => b.CheckInWeight);

            return Ok(totalWeight);
        }

        // GET: api/Baggages/flight/{flightId}/cabinweight
        [HttpGet("flight/{flightId}/cabinweight")]
        public async Task<ActionResult<decimal>> GetTotalCabinWeightByFlight(int flightId)
        {
            var totalWeight = await _context.Baggages
                .Where(b => b.FlightId == flightId)
                .SumAsync(b => b.CabinWeight);

            return Ok(totalWeight);
        }

       // Get Baggages By Passenger And Flight

        // GET: api/Baggages/passenger/{passengerId}/flight/{flightId}
        [HttpGet("passenger/{passengerId}/flight/{flightId}")]
        public async Task<ActionResult<IEnumerable<Baggage>>> GetBaggagesByPassengerAndFlight(int passengerId, int flightId)
        {
            return await _context.Baggages
                .Where(b => b.PassengerId == passengerId && b.FlightId == flightId)
                .ToListAsync();
        }

        //Validating the cabin Weight

        // GET: api/Baggages/flight/{flightId}/validatecabinweight
        [HttpGet("flight/{flightId}/validatecabinweight")]
        public async Task<ActionResult<bool>> ValidateCabinWeight(int flightId)
        {
            var totalWeight = await _context.Baggages
                .Where(b => b.FlightId == flightId)
                .SumAsync(b => b.CabinWeight);

            return Ok(totalWeight <= 50); //in kgs
        }





    }
}
