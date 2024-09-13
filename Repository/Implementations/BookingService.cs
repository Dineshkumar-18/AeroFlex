using AeroFlex.Data;
using AeroFlex.Dtos;
using AeroFlex.Models;
using AeroFlex.Repository.Contracts;
using AeroFlex.Response;
using Microsoft.EntityFrameworkCore;

namespace AeroFlex.Repository.Implementations
{
    public class BookingService(ApplicationDbContext _context) : IBookingService
    {
       public async Task<GeneralResponse> CreateBookingAsync(BookingDto bookingDTO, int UserId,int FlightScheduleId)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var flightPricing = await _context.FlightsPricings.FirstOrDefaultAsync(fp=>fp.FlightScheduleId== FlightScheduleId);

                    decimal totalPrice = 0m;

                  
                      foreach (var kvp in bookingDTO.SeatAllocations)
                        {
                            // Assuming SeatPrice exists in PassengerDto
                            var seat = await _context.Seats.FirstOrDefaultAsync(s=>s.SeatNumber==kvp.Key);
                            if (seat is null) return new GeneralResponse(false, $"Seat Number {kvp.Key} is wrong");
                            if(seat.Status.ToString().ToLower()=="booked")
                            {
                                return new GeneralResponse(false, $"Seat Number {kvp.Key} is already booked");
                            }
                            totalPrice += seat.SeatPrice;
                        }

                    var booking = new Booking
                    {
                        UserId = UserId,
                        FlightScheduleId =FlightScheduleId,
                        TotalPassengers = bookingDTO.SeatAllocations.Count,
                        BookingDate = DateTime.UtcNow,
                        FlightPricingId = flightPricing!.FlightPricingId,
                        TotalAmount = totalPrice+flightPricing.Totalprice,
                        BookingStatus = Bookingstatus.PENDING,
                    };

                    _context.Bookings.Add(booking);
                    await _context.SaveChangesAsync();
                    //passenger-seat allocation

                   
                        foreach (var kvp in bookingDTO.SeatAllocations)
                        {
                            // Assuming SeatPrice exists in PassengerDto
                            var seat = await _context.Seats.FirstOrDefaultAsync(s => s.SeatNumber == kvp.Key);
                            if (seat is null) return new GeneralResponse(false, $"Seat Number {kvp.Key} is wrong");
                        if (seat.Status.ToString().ToLower() == "booked")
                        {
                            return new GeneralResponse(false, $"Seat Number {kvp.Key} is already booked");
                        }


                        var Passenger = new Passenger
                            {
                                Firstname = kvp.Value.Firstname,
                                Lastname = kvp.Value.Lastname,
                                Gender = kvp.Value.Gender,
                                DateOfBirth=kvp.Value.DateOfBirth,
                                PassengerType = kvp.Value.PassengerType,
                                BookingId = booking.BookingId,
                                PassengerStatus=PassengerStatus.CONFIRMED
                            };

                            _context.Passengers.Add(Passenger);
                            await _context.SaveChangesAsync();

                            seat.Status = SeatStatus.RESERVED;
                            seat.BookingId= booking.BookingId;
                            seat.PassengerId=Passenger.PassengerId;

                            _context.Seats.Update(seat);
                            await _context.SaveChangesAsync();
                        }
                    await transaction.CommitAsync();
                    return new GeneralResponse(true, "Booked successfully");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new GeneralResponse(false,"Error Occured while booking");
                }
            }
        }

        public Task<Booking> GetBookingByIdAsync(int bookingId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Booking>> GetUserBookingsAsync(int userId)
        {
            throw new NotImplementedException();
        }
    }
}
