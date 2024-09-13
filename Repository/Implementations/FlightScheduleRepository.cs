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

        public async Task<IEnumerable<FlightScheduleDTO>> GetFlightSchedulesAsync(string departureAirport, string arrivalAirport, DateTime date)
        {
            var departureAirportId = await context.Airports
        .FirstOrDefaultAsync(a => a.AirportName == departureAirport);

            var arrivalAirportId = await context.Airports
                .FirstOrDefaultAsync(a => a.AirportName == arrivalAirport);

            if (departureAirportId == null || arrivalAirportId == null)
            {
                return Enumerable.Empty<FlightScheduleDTO>();
            }

            // Retrieve FlightSchedules with date comparison only on the Date part
            return await context.FlightsSchedules
                .Include(fs => fs.Flight)
                .Include(fs => fs.DepartureAirport)
                .Include(fs => fs.ArrivalAirport)
                .Where(fs => fs.DepartureAirportId == departureAirportId.AirportId
                          && fs.ArrivalAirportId == arrivalAirportId.AirportId
                          && fs.FlightStatus==FlightStatus.SCHEDULED
                          && EF.Functions.DateDiffDay(fs.DepartureTime, date) == 0) // Date comparison
                .Select(fs => new FlightScheduleDTO
                {
                    FlightNumber = fs.Flight.FlightNumber,
                    DepartureAirport = fs.DepartureAirport.AirportName,
                    ArrivalAirport = fs.ArrivalAirport.AirportName,
                    DepartTime = fs.DepartureTime,
                    ArrivalTime = fs.ArrivalTime
                }).ToListAsync();
        }
    }
}
