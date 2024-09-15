using AeroFlex.Data;
using AeroFlex.Dtos;
using AeroFlex.Models;
using AeroFlex.Repository.Contracts;
using AeroFlex.Response;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.Xml;

namespace AeroFlex.Repository.Implementations
{
    public class CancellationRepository(ApplicationDbContext context) : ICancellationRepository
    {
        public Task<CancellationDto> CancellationHistoryByUser(int UserId)
        {
            throw new NotImplementedException();
        }

        public async Task<GeneralResponse> CancellationProcess(Cancel cancellation, int userId, int flightScheduleId)
        {
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                decimal TotalRefundAmount = 0m;
                //for accurately storing the Cancelledtime inrespective of the running time
                var CancelledTime = DateTime.Now;
                var cancellationFee = await context.CancellationFees
                    .FirstOrDefaultAsync(cf => cf.FlightScheduleId == flightScheduleId);
                if (cancellationFee == null) return new GeneralResponse(false, "CancellationFee information not found");

                var flightPricings = await context.FlightsPricings
                    .FirstOrDefaultAsync(fp => fp.FlightScheduleId == flightScheduleId);
                if (flightPricings == null) return new GeneralResponse(false, "Flight pricings not found");

                var booking = await context.Passengers.Include(p => p.Booking)
                                     .FirstOrDefaultAsync(p => p.PassengerId == cancellation.Passengers[0]);
                if (booking == null) return new GeneralResponse(false, "Booking information not found");

                var IndividualFlightPricing = flightPricings.Totalprice / booking.Booking.TotalPassengers;

                var cancellationInfos = new List<CancellationInfo>();

                foreach (var passengerId in cancellation.Passengers)
                {
                    var passenger = await context.Passengers.Include(p => p.Seat)
                        .FirstOrDefaultAsync(p => p.PassengerId == passengerId);
                    if (passenger == null) return new GeneralResponse(false, $"Passenger id {passengerId} not found");

                    var seat = passenger.Seat;
                    if (seat == null) return new GeneralResponse(false, "Seat information not found");

                    decimal refundAmount = 0m;

                    if (cancellationFee.ApplicableDueDate > CancelledTime)
                    {
                        refundAmount = IndividualFlightPricing + seat.SeatPrice;
                        TotalRefundAmount = TotalRefundAmount + refundAmount;
                    }

                    var cancellationInfo = new CancellationInfo
                    {
                        CancellationFeeId = cancellationFee.CancellationFeeId,
                        PassengerId = passengerId,
                        SeatId = seat.SeatId,
                        CancelledTime= CancelledTime,
                        FlightScheduleId = flightScheduleId,
                        RefundAmount = refundAmount,
                        CancellationReason = cancellation.CancellationReason,
                    };

                    cancellationInfos.Add(cancellationInfo);

                    // Update passenger status
                    passenger.PassengerStatus = PassengerStatus.CANCELLED;
                    seat.Status = SeatStatus.AVAILABLE;
                    seat.BookingId = null;
                    seat.PassengerId = null;
                    context.Passengers.Update(passenger);
                }

                // Add cancellation info and save changes to generate CancellationId
                await context.CancellationInfos.AddRangeAsync(cancellationInfos);
                await context.SaveChangesAsync();

                // Update booking status
                var totalPassengers = booking.Booking.TotalPassengers;
                booking.Booking.TotalPassengers = totalPassengers - cancellation.Passengers.Count;

                if (booking.Booking.TotalPassengers > 0)
                {
                    booking.Booking.BookingStatus = Bookingstatus.PARTIALLY_CANCELLED;
                }
                else
                {
                    booking.Booking.BookingStatus = Bookingstatus.CANCELLED;
                }

                context.Bookings.Update(booking.Booking);
                await context.SaveChangesAsync();


                if (cancellationFee.ApplicableDueDate < CancelledTime)
                {
                    await transaction.CommitAsync();
                    return new GeneralResponse(true, "Your booking has cancelled successfully but you are not eligible for getting refund amount");
                }

                decimal ChargeRateDistribution = cancellationFee.ChargeRate / cancellation.Passengers.Count;
                TotalRefundAmount = TotalRefundAmount - ((TotalRefundAmount * (ChargeRateDistribution) / 100));
                 context.Refunds.Add(new Refund
                 {
                     RefundAmount = TotalRefundAmount,
                     BookingId = booking.Booking.BookingId,
                     UserId = userId,
                 });

                await context.SaveChangesAsync();
                await transaction.CommitAsync();
   
                return new GeneralResponse(true, "Successfully cancelled your booking, refund will be processed after verifications.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new GeneralResponse(false, "Error occurred while cancelling the booking.");
            }
        }



        public Task<CancellationDto> ViewCancellationDetails(int flightScheduleId)
        {
            throw new NotImplementedException();
        }
    }
}
