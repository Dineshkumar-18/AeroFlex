using AeroFlex.Data;
using AeroFlex.Dtos;
using AeroFlex.Models;
using AeroFlex.Repository.Contracts;
using Microsoft.EntityFrameworkCore;
using System;

namespace AeroFlex.Repository.Implementations
{
    public class FlightScheduleRepository(ApplicationDbContext context) : IFlightScheduleRepository
    {

        public async Task<List<FlightScheduleDTO>> GetFlightSchedulesAsync(int departureAirport, int arrivalAirport, DateOnly date, string Class, int TotalPassengers)
        {
            var departureAirportId = await context.Airports
           .FirstOrDefaultAsync(a => a.AirportId == departureAirport);

            var arrivalAirportId = await context.Airports
                .FirstOrDefaultAsync(a => a.AirportId == arrivalAirport);

            if (departureAirportId == null || arrivalAirportId == null)
            {
                return new List<FlightScheduleDTO>();
            }

            var ClassId=await context.Classes.FirstOrDefaultAsync(c=>c.ClassName==Class);


            // Retrieve FlightSchedules with date comparison only on the Date part


            var results = await (from fs in context.FlightsSchedules
                                 join flight in context.Flights
                                 on fs.FlightId equals flight.FlightId
                                 join airline in context.Airlines
                                 on flight.AirlineId equals airline.AirlineId
                                 join dairport in context.Airports
                                 on fs.DepartureAirportId equals dairport.AirportId
                                 join aairport in context.Airports
                                 on fs.ArrivalAirportId equals aairport.AirportId
                                 join fsc in context.FlightScheduleClasses
                                 on fs.FlightScheduleId equals fsc.FlightScheduleId
                                 join cls in context.Classes
                                 on fsc.ClassId equals cls.ClassId
                                 join flightpricing in context.FlightsPricings
                                 on fs.FlightScheduleId equals flightpricing.FlightScheduleId
                                 join seat in context.Seats
                                 on fsc.FlightclassId equals seat.FlightScheduleClassId
                                 where dairport.AirportId == departureAirportId.AirportId &&
                                 aairport.AirportId == arrivalAirportId.AirportId &&
                                 DateOnly.FromDateTime(fs.DepartureTime)==date &&
                                 cls.ClassId == ClassId.ClassId &&
                                 seat.Status == SeatStatus.AVAILABLE
                                 group seat by new
                                 {
                                     fs.FlightScheduleId,
                                     departure=dairport.AirportName,
                                     arrival=aairport.AirportName,
                                     departureIata=dairport.IataCode,
                                     arrivalIata=aairport.IataCode,
                                     cls.ClassName,
                                     airline.AirlineName,
                                     fs.DepartureTime,
                                     fs.ArrivalTime,
                                     flight.FlightNumber,
                                     flightpricing.Totalprice,
                                     flightpricing.TaxAmount,
                                     departureCity=dairport.City,
                                     arrivalCity=aairport.City,
                                     airline.AirlineLogo,
                                 } into seatGroup
                                 where seatGroup.Count() >= TotalPassengers // Check if available seats >= total passengers
                                 select new FlightScheduleDTO
                                 {
                                     FlightScheduleId=seatGroup.Key.FlightScheduleId,
                                     AirlineName = seatGroup.Key.AirlineName,
                                     DepartureAirportIataCode = seatGroup.Key.departureIata,
                                     ArrivalAirportIataCode = seatGroup.Key.arrivalIata,
                                     FlightNumber = seatGroup.Key.FlightNumber,
                                     DepartureAirport = seatGroup.Key.departure,
                                     DepartureCity = seatGroup.Key.departureCity,
                                     ArrivalCity = seatGroup.Key.arrivalCity,
                                     ArrivalAirport = seatGroup.Key.arrival,
                                     DepartureDate=DateOnly.FromDateTime(seatGroup.Key.DepartureTime),
                                     ArrivalDate= DateOnly.FromDateTime(seatGroup.Key.ArrivalTime),
                                     DepartureTime = TimeOnly.FromDateTime(seatGroup.Key.DepartureTime),
                                     ArrivalTime = TimeOnly.FromDateTime(seatGroup.Key.ArrivalTime),
                                     Duration = TimeOnly.FromTimeSpan(seatGroup.Key.ArrivalTime - seatGroup.Key.DepartureTime),
                                     FlightPricings = seatGroup.Key.Totalprice,
                                     TaxCharges = seatGroup.Key.TaxAmount,
                                     AirlineImagePath= seatGroup.Key.AirlineLogo,
                                     AvailableSeatsCount = seatGroup.Count() // The count of available seats
                                 }).ToListAsync();


            return results;
        }
    }
}
