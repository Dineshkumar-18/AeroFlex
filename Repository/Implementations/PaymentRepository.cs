using AeroFlex.Data;
using AeroFlex.Dtos;
using AeroFlex.Models;
using AeroFlex.Repository.Contracts;
using AeroFlex.Response;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace AeroFlex.Repository.Implementations
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ITicketRepository _ticketRepository;

        public PaymentRepository(ApplicationDbContext context,ITicketRepository ticketRepository)
        {
            _context = context;
            _ticketRepository = ticketRepository;
        }

        public async Task<GeneralResponse<PaymentDetailsByFlightSchduleDto>> GetPaymentInfo(int flightscheduleId)
        {


            var basicInfo = await (from flight in _context.Flights
                                   join fs in _context.FlightsSchedules on flight.FlightId equals fs.FlightId
                                   where fs.FlightScheduleId == flightscheduleId
                                   select new
                                   {
                                       FlightNumber = flight.FlightNumber,
                                       DepartureTime = fs.DepartureTime,
                                       ArrivalTime = fs.ArrivalTime,
                                       TotalSeatsAvailable = fs.Flight.TotalSeats,
                                       TotalBookings = fs.Bookings.Count(),
                                       TotalSeatsBooked = fs.Seats.Count(s => s.Status == SeatStatus.BOOKED),
                                   }).SingleOrDefaultAsync();

            if (basicInfo == null)
            {
                // Log or debug the basicInfo to see if data is present
                return new GeneralResponse<PaymentDetailsByFlightSchduleDto>(false, "Basic flight schedule data not found", null);
            }




            var paymentInfo = await (from flight in _context.Flights
                                     join fs in _context.FlightsSchedules
                                     on flight.FlightId equals fs.FlightId
                                     join booking in _context.Bookings
                                     on fs.FlightScheduleId equals booking.FlightScheduleId
                                     join seat in _context.Seats
                                     on fs.FlightScheduleId equals seat.FlightScheduleId
                                     join passenger in _context.Passengers
                                     on booking.BookingId equals passenger.BookingId
                                     join payment in _context.Payments
                                     on booking.BookingId equals payment.BookingId
                                     join fp in _context.FlightsPricings
                                     on fs.FlightScheduleId equals fp.FlightScheduleId
                                     where fs.FlightScheduleId == flightscheduleId
                                     select new PaymentDetailsByFlightSchduleDto
                                     {
                                         FlightNumber = flight.FlightNumber,
                                         DepartureTime = fs.DepartureTime,
                                         ArrivalTime = fs.ArrivalTime,
                                         Origin = fs.DepartureAirport.City + "-" + fs.DepartureAirport.IataCode,
                                         Destination = fs.ArrivalAirport.City + "-" + fs.ArrivalAirport.IataCode,
                                         TotalSeatsAvailable = fs.Flight.TotalSeats,
                                         TotalTicketsSold = fs.Seats.Where(s => s.Status == SeatStatus.BOOKED).Count(),
                                         TotalRevenue = fs.Bookings.Where(b => b.Payment != null && b.Payment.PaymentStatus == PaymentStatus.SUCCESS).Sum(b => b.Payment.PaidAmount)+ (fs.CancellationInfos.Sum(ci => ci.CancellationCharge)),
                                         TaxAmount = fs.Bookings.Sum(b => b.TaxAmount),
                                         DiscountsApplied = (decimal)fp.Discount,
                                         TotalDeductions = (decimal)(fs.Bookings.Sum(b => b.TaxAmount) + fp.Discount),
                                         NetRevenue = ((fs.Bookings.Where(b => b.Payment != null && b.Payment.PaymentStatus == PaymentStatus.SUCCESS).Sum(b => b.Payment.PaidAmount))+ (fs.CancellationInfos.Sum(ci => ci.CancellationCharge))) - ((decimal)(fs.Bookings.Sum(b => b.TaxAmount) + fp.Discount)),
                                         CommissionFee= fs.Bookings.Where(b => b.Payment != null && b.Payment.PaymentStatus == PaymentStatus.SUCCESS).Sum(b => b.Payment.PaidAmount*0.05m),
                                         CancellatonChargesRevenue = fs.CancellationInfos.Sum(ci => ci.CancellationCharge),
                                         RefundsIssued=fs.Bookings.Where(b=>b.Refunds!=null && b.Refunds.Any()).SelectMany(b=>b.Refunds).Sum(r=>r.RefundAmount),

                                         FinalAmountPayable= ((fs.Bookings.Where(b => b.Payment != null && b.Payment.PaymentStatus == PaymentStatus.SUCCESS).Sum(b => b.Payment.PaidAmount)) + (fs.CancellationInfos.Sum(ci => ci.CancellationCharge))) - ((decimal)(fs.Bookings.Sum(b => b.TaxAmount) + fp.Discount))-(fs.Bookings.Where(b => b.Payment != null && b.Payment.PaymentStatus == PaymentStatus.SUCCESS).Sum(b => b.Payment.PaidAmount * 0.05m))-(fs.Bookings.Where(b => b.Refunds != null && b.Refunds.Any()).SelectMany(b => b.Refunds).Sum(r => r.RefundAmount))
                                     }).SingleOrDefaultAsync();
            if (paymentInfo == null) return new GeneralResponse<PaymentDetailsByFlightSchduleDto>(false, "Payment report not found",null);
            return new GeneralResponse<PaymentDetailsByFlightSchduleDto>(true, "Successfully retrieved the payment report", paymentInfo);
            
        }

        public async Task<GeneralResponse<List<TicketDto>>> PaymentSucessTicketGenerate(int paymentId)
        {
            var payment=await _context.Payments.FirstOrDefaultAsync(p=>p.PaymentId==paymentId && p.PaymentStatus==PaymentStatus.SUCCESS);
            if (payment == null) return new GeneralResponse<List<TicketDto>>(false, "Payment not made or payment is pending");

            var flightScheduleId = await (from pay in _context.Payments
                                    join booking in _context.Bookings
                                    on pay.BookingId equals booking.BookingId
                                    where pay.PaymentId == paymentId
                                    select booking.FlightScheduleId
                                    ).FirstOrDefaultAsync();
    
            var tickets = await _ticketRepository.GenerateTicket(flightScheduleId, payment.BookingId);
            return new GeneralResponse<List<TicketDto>>(true, "Payment successfully made and ticket being generated...",tickets);
        }

        public async Task<List<TicketDto>> ProcessPaymentAsync(PaymentDto paymentDto)
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
                        ReferenceNumber = paymentDto.ReferenceNumber,
                        PaidAmount =  paymentDto.PaidAmount,
                        BalanceAmount = 0,
                        PaymentDate = paymentDto.PaymentDate,
                        PaymentMethod = paymentDto.PaymentMethod,
                        PaymentStatus = PaymentStatus.SUCCESS,
                    };

                    // Save the payment record
                    await _context.Payments.AddAsync(payment);

                    // Update the booking status for each passenger

                    var tickets = new List<Ticket>();
                    foreach (var passenger in booking.Passengers)
                    {
                        passenger.PassengerStatus = PassengerStatus.CONFIRMED;
                        passenger.Seat.Status = SeatStatus.BOOKED; // Change seat status from RESERVED to BOOKED

                        var ticket = new Ticket
                        {
                            PassengerId = passenger.PassengerId,
                            BookingId=passenger.BookingId,
                            SeatId = passenger.Seat.SeatId,
                            TicketPrice = passenger.Seat.SeatPrice,
                            TicketStatus = Bookingstatus.CONFIRMED
                        };
                        tickets.Add(ticket);
                    }


                    // Update the booking status to CONFIRMED
                    booking.BookingStatus = Bookingstatus.CONFIRMED;

                    await _context.Tickets.AddRangeAsync(tickets);

                    //// Update seat status for all seats in this booking
                    //foreach (var seat in booking.Seats)
                    //{
                    //    seat.Status = SeatStatus.BOOKED; // Set seat status to BOOKED
            


                        // Save changes to the database
                     await _context.SaveChangesAsync();

                    // Commit the transaction
                    await transaction.CommitAsync();


                    var ticketInfo=await _ticketRepository.GenerateTicket(booking.FlightScheduleId, paymentDto.BookingId);



                    // Return the processed payment DTO
                    return ticketInfo;
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
