using AeroFlex.Data;
using AeroFlex.Dtos;
using AeroFlex.Models;
using AeroFlex.Repository.Contracts;
using Microsoft.EntityFrameworkCore;

namespace AeroFlex.Repository.Implementations
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ApplicationDbContext _context;

        public PaymentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PaymentDto> ProcessPaymentAsync(PaymentDto paymentDto)
        {
            // Start a database transaction
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Fetch the booking details
                    var booking = await _context.Bookings
                        .Include(b => b.FlightSchedule)
                        .Include(b => b.Passengers)
                        .Include(b => b.Seats)
                        .FirstOrDefaultAsync(b => b.BookingId == paymentDto.BookingId);

                    if (booking == null)
                    {
                        throw new Exception("Booking not found.");
                    }

                    // Check if the paid amount matches the total booking amount
                    if (paymentDto.PaidAmount != booking.TotalAmount)
                    {
                        throw new Exception("Paid amount does not match the total booking amount.");
                    }

                    // Create the Payment entity
                    var payment = new Payment
                    {
                        BookingId = paymentDto.BookingId,
                        ReferenceId = paymentDto.ReferenceId,
                        PaidAmount = (int)paymentDto.PaidAmount,
                        BalanceAmount = 0,
                        PaymentDate = paymentDto.PaymentDate,
                        PaymentMethod = paymentDto.PaymentMethod,
                        PaymentStatus = PaymentStatus.SUCCESS,
                        Booking = booking
                    };

                    // Save the payment record
                    await _context.Payments.AddAsync(payment);

                    // Update the booking status for each passenger
                    foreach (var passenger in booking.Passengers)
                    {
                        passenger.Seat.Status = SeatStatus.BOOKED; // Change seat status from RESERVED to BOOKED
                    }


                    // Update the booking status to CONFIRMED
                    booking.BookingStatus = Bookingstatus.CONFIRMED;

                    // Update seat status for all seats in this booking
                    foreach (var seat in booking.Seats)
                    {
                        seat.Status = SeatStatus.BOOKED; // Set seat status to BOOKED
                    }

                    // Save changes to the database
                    await _context.SaveChangesAsync();

                    // Commit the transaction
                    await transaction.CommitAsync();

                    // Return the processed payment DTO
                    return new PaymentDto
                    {
                        BookingId = paymentDto.BookingId,
                        ReferenceId = paymentDto.ReferenceId,
                        PaidAmount = paymentDto.PaidAmount,
                        PaymentMethod = paymentDto.PaymentMethod,
                        PaymentDate = paymentDto.PaymentDate
                    };
                }
                catch (Exception ex)
                {
                    // Rollback the transaction if any error occurs
                    await transaction.RollbackAsync();
                    throw new Exception($"Payment processing failed: {ex.Message}");
                }
            }
        }
    }


}
