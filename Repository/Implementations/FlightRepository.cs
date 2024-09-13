using AeroFlex.Data;
using AeroFlex.Dtos;
using AeroFlex.Models;
using AeroFlex.Response;
using AeroFlex.Repository.Contracts;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.Extensions.Logging.Abstractions;

namespace AeroFlex.Repository.Implementations
{
    public class FlightRepository : IFlight
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly ApplicationDbContext _context;
        public FlightRepository(ApplicationDbContext context, IHttpContextAccessor httpContext)
        {
            _context = context;
            _httpContext = httpContext;
        }


        public async Task<GeneralResponse> AddFlightSchedule(FlightScheduleDTO FlightSchedule, int flightId)
        {
            if (FlightSchedule == null) return new GeneralResponse(false, "Model is invalid");

            var DepartAirport = await _context.Airports.FirstOrDefaultAsync(a => a.AirportName == FlightSchedule.DepartureAirport);
            var ArrivalAirport = await _context.Airports.FirstOrDefaultAsync(a => a.AirportName == FlightSchedule.ArrivalAirport);

            var flight = await _context.FlightsSchedules.Where(fs => fs.FlightId == flightId).ToListAsync();

            if (flight.Any(f => f.ArrivalTime.AddHours(1) > FlightSchedule.DepartTime))
            {
                return new GeneralResponse(false, "Departure time should be at least 1 hour after the last flight's arrival time.");
            }


            var SameTimeFlightSchedule = _context.FlightsSchedules
          .Where(fs => fs.DepartureAirportId == DepartAirport.AirportId
              && fs.ArrivalAirportId == ArrivalAirport.AirportId
              && fs.Duration == (FlightSchedule.ArrivalTime - FlightSchedule.DepartTime))
          .AsEnumerable() // Client-side evaluation from here
          .FirstOrDefault(fs => Math.Abs((fs.DepartureTime - FlightSchedule.DepartTime).TotalMinutes) < 1 // Allow a margin of 1 minute
              && Math.Abs((fs.ArrivalTime - FlightSchedule.ArrivalTime).TotalMinutes) < 1);

            if (SameTimeFlightSchedule is not null) return new GeneralResponse(false, "Cannot schedule because already one flight has scheduled");

            var flightSchedule = new FlightSchedule
            {
                FlightId = flightId,
                DepartureAirportId = DepartAirport.AirportId,
                ArrivalAirportId = ArrivalAirport.AirportId,
                DepartureTime = FlightSchedule.DepartTime,
                ArrivalTime = FlightSchedule.ArrivalTime,
                Duration = (FlightSchedule.ArrivalTime - FlightSchedule.DepartTime)
            };

            _context.FlightsSchedules.Add(flightSchedule);
            await _context.SaveChangesAsync();


            var flightNumber = await _context.Flights.FirstOrDefaultAsync(f => f.FlightId == flightId);

            return new GeneralResponse(true, $"FlightNumber {flightNumber.FlightNumber} has been scheduled successfully");


        }

        public async Task<GeneralResponse> AddFlight(AddFlightDto addFlight, int AirlineId)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    if (addFlight == null) return new GeneralResponse(false, "Model is invalid");

                    var airline = await _context.Airlines.FirstOrDefaultAsync(a => a.AirlineId == AirlineId);
                    if (airline == null) return new GeneralResponse(false, "Airline is not found");

                    var DepartAirport = await _context.Airports.FirstOrDefaultAsync(a => a.AirportName == addFlight.DepartAirport);
                    var ArrivalAirport = await _context.Airports.FirstOrDefaultAsync(a => a.AirportName == addFlight.ArrivalAirport);

                    if (DepartAirport == null || ArrivalAirport == null) return new GeneralResponse(false, "Airport not found");

                    var flight = _context.Flights.Add(

                        new Flight()
                        {
                            FlightNumber = addFlight.FlightNumber,
                            AirCraftType = addFlight.AirCraftType,
                            FlightType = (TravelType)Enum.Parse(typeof(TravelType), addFlight.FlightType.ToUpper()),
                            DepartureAirportId = DepartAirport.AirportId,
                            ArrivalAirportId = ArrivalAirport.AirportId,
                            TotalSeats = addFlight.TotalSeats,
                            AirlineId = AirlineId,
                            TotalSeatColumn = addFlight.TotalSeatColumn
                        });
                    await _context.SaveChangesAsync();

                    var flightOwner = await _context.FlightOwners.FirstOrDefaultAsync(fo=>fo.UserId==airline.FlightOwnerId);
                    if (flightOwner == null)
                    {
                        throw new Exception("Flight owner not found");
                    }

                    flightOwner.TotalFlightsManaged += 1;
                    _context.FlightOwners.Update(flightOwner);
                    await _context.SaveChangesAsync();

                    // Step 3: Commit the transaction if all operations succeed
                    await transaction.CommitAsync();

                    return new GeneralResponse(true, "Flight created Successfully");
                }
                catch (Exception ex)
                {

                    await transaction.RollbackAsync();
                    return new GeneralResponse(false,"Error while creating the flight");
                }
            }


        }
    }
}

