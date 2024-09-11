using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AeroFlex.Data;
using AeroFlex.Models;

namespace AeroFlex.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BookingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Bookings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookings()
        {
            return await _context.Bookings.ToListAsync();
        }

        // GET: api/Bookings/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Booking>> GetBooking(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);

            if (booking == null)
            {
                return NotFound();
            }

            return booking;
        }

        // PUT: api/Bookings/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBooking(int id, Booking booking)
        {
            if (id != booking.BookingId)
            {
                return BadRequest();
            }

            _context.Entry(booking).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookingExists(id))
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

        // POST: api/Bookings
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Booking>> PostBooking(Booking booking)
        {
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBooking", new { id = booking.BookingId }, booking);
        }

        // DELETE: api/Bookings/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.BookingId == id);
        }


        //New Business logic codes


        //Check Booking Capacity Before Booking the flight

        // POST: api/Bookings
        [HttpPost]
        public async Task<ActionResult<Booking>> BeforeBooking(Booking booking)
        {
            var availableSeats = await _context.Seats
                .Where(s => s.FlightScheduleId == booking.FlightScheduleId && s.Status == SeatStatus.AVAILABLE)
                .CountAsync();

             if (availableSeats < booking.TotalPassengers)
            {
                return BadRequest("Not enough seats available for the selected flight schedule.");
            }

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBooking", new { id = booking.BookingId }, booking);
        }

        //This method is to avoid duplicate booking
        // POST: api/Bookings
        [HttpPost]
        public async Task<ActionResult<Booking>> BookingValidation(Booking booking)
        {
            var existingBooking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.UserId == booking.UserId && b.FlightScheduleId == booking.FlightScheduleId);

            if (existingBooking != null)
            {
                return BadRequest("A booking already exists for this user and flight.");
            }

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBooking", new { id = booking.BookingId }, booking);
        }


        //Update Booking status based on payment

        [HttpPut("updatestatus/{bookingId}")]

        public async Task<ActionResult> UpdateBookingStatusAfterPayment(int bookingId, decimal paidAmount)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);

            if (booking == null)
            {
                return NotFound("Booking not found.");
            }

            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.BookingId == bookingId);

            if (payment != null)
            {
                if (paidAmount >= booking.TotalAmount)
                {
                    booking.BookingStatus = Bookingstatus.CONFIRMED;
                }
                else
                {
                    booking.BookingStatus = Bookingstatus.PARTIALLY_CANCELLED;
                }

                _context.Entry(booking).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok("Booking status updated.");
            }

            return BadRequest("Payment not found for the booking.");
        }

        //cancel booking and apply cancellation fee

        [HttpPost("cancel/{id}")]
        public async Task<ActionResult> CancelBooking(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);

            if (booking == null)
                {
                return NotFound();
            }

            // Logic to calculate cancellation fees
            var cancellationFee = await _context.CancellationFees
                .FirstOrDefaultAsync(f => f.FlightScheduleId == booking.FlightScheduleId);

            if (cancellationFee == null)
            {
                return BadRequest("Cancellation fee structure not found.");
            }

            decimal refundAmount = booking.TotalAmount - cancellationFee.ChargeRate;

            booking.BookingStatus = Bookingstatus.CANCELLED;

            // Create a refund record
            var refund = new Refund
            {
                CancellationId = booking.BookingId,
                RefundAmount = refundAmount,
                RefundDate = DateTime.UtcNow,
                RefundReason = "Customer Cancelled",
                RefundStatus = (PaymentStatus) 1 // Assuming 1 means 'Pending'
            };

            _context.Refunds.Add(refund);
            _context.Entry(booking).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return Ok("Booking cancelled and refund processed.");
        }


        //Get bookings by Flight Schedule

        // GET: api/Bookings/flightSchedule/{flightScheduleId}
        [HttpGet("flightSchedule/{flightScheduleId}")]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookingsByFlightSchedule(int flightScheduleId)
        {
            return await _context.Bookings
                .Where(b => b.FlightScheduleId == flightScheduleId)
                .ToListAsync();
        }

        //Booking history for the user

        [HttpGet("history/{userId}")]
        public async Task<ActionResult<IEnumerable<Booking>>> GetUserBookingHistory(int userId)
        {
            var bookingHistory = await _context.Bookings
                .Where(b => b.UserId == userId)
                .Include(b => b.FlightSchedule)
                .Include(b => b.Ticket)
                .ToListAsync();

            if (bookingHistory == null || !bookingHistory.Any())
            {
                return NotFound("No booking history found for this user.");
            }

            return Ok(bookingHistory);
        }















    }
}
