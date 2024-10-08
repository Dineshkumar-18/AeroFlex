﻿using AeroFlex.Data;
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

        public async Task<GeneralResponse> AddFlightSchedule(AddFlightScheduleDto FlightSchedule, int flightId)
          {
            if (FlightSchedule == null) return new GeneralResponse(false, "Model is invalid");

            var DepartAirport = await _context.Airports.FirstOrDefaultAsync(a => a.AirportId == FlightSchedule.DepartureAirportId);
            var ArrivalAirport = await _context.Airports.FirstOrDefaultAsync(a => a.AirportId == FlightSchedule.ArrivalAirportId);

            var flight = await _context.FlightsSchedules.Where(fs => fs.FlightId == flightId).ToListAsync();

            var latestArrivalTime = flight.Max(f => f.ArrivalTime);

            if (latestArrivalTime.AddHours(1) > FlightSchedule.DepartureTime)
            {
                return new GeneralResponse(false, "Departure time should be at least 1 hour after the last flight's arrival time.");
            }


           var SameTimeFlightSchedule = _context.FlightsSchedules
          .Where(fs => fs.DepartureAirportId == DepartAirport.AirportId
              && fs.ArrivalAirportId == ArrivalAirport.AirportId    
              && fs.Duration == (FlightSchedule.ArrivalTime - FlightSchedule.DepartureTime))
          .AsEnumerable() // Client-side evaluation from here
          .FirstOrDefault(fs => Math.Abs((fs.DepartureTime - FlightSchedule.DepartureTime).TotalMinutes) < 1 // Allow a margin of 1 minute
              && Math.Abs((fs.ArrivalTime - FlightSchedule.ArrivalTime).TotalMinutes) < 1);

            if (SameTimeFlightSchedule is not null) return new GeneralResponse(false, "Cannot schedule because already one flight has scheduled");

            var flightSchedule = new FlightSchedule
            {
                FlightId = flightId,
                DepartureAirportId = DepartAirport.AirportId,
                ArrivalAirportId = ArrivalAirport.AirportId,
                DepartureTime = FlightSchedule.DepartureTime,
                ArrivalTime = FlightSchedule.ArrivalTime,
                Duration = (FlightSchedule.ArrivalTime - FlightSchedule.DepartureTime)
            };

            _context.FlightsSchedules.Add(flightSchedule);
            await _context.SaveChangesAsync();


            var flightNumber = await _context.Flights.FirstOrDefaultAsync(f => f.FlightId == flightId);

            return new GeneralResponse(true, $"FlightNumber {flightNumber.FlightNumber} has been scheduled successfully");


        }

        public async Task<GeneralResponse<object>> AddFlight(AddFlightDto addFlight, int AirlineId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (addFlight == null) return new GeneralResponse<object>(false, "Model is invalid");

                var airline = await _context.Airlines.FirstOrDefaultAsync(a => a.AirlineId == AirlineId);
                if (airline == null) return new GeneralResponse<object>(false, "Airline is not found");

                var DepartAirport = await _context.Airports.FirstOrDefaultAsync(a => a.AirportId == addFlight.DepartAirport);
                var ArrivalAirport = await _context.Airports.FirstOrDefaultAsync(a => a.AirportId == addFlight.ArrivalAirport);

                if (DepartAirport == null || ArrivalAirport == null) return new GeneralResponse<object>(false, "Airport not found");

                var flightEntry = await _context.Flights.AddAsync(

                    new Flight()
                    {
                        FlightNumber = addFlight.FlightNumber,
                        AirCraftType = addFlight.AirCraftType,
                        FlightType = (TravelType)Enum.Parse(typeof(TravelType), addFlight.FlightType.ToUpper()),
                        DepartureAirportId = addFlight.DepartAirport,
                        ArrivalAirportId = addFlight.ArrivalAirport,
                        TotalSeats = addFlight.TotalSeats,
                        AirlineId = AirlineId,
                        TotalSeatColumn = addFlight.TotalSeatColumn
                    });

                
                await _context.SaveChangesAsync();

                var flight = flightEntry.Entity;

                var flightOwner = await _context.FlightOwners.FirstOrDefaultAsync(fo => fo.UserId == airline.FlightOwnerId);
                if (flightOwner == null)
                {
                    throw new Exception("Flight owner not found");
                }

                flightOwner.TotalFlightsManaged += 1;
                _context.FlightOwners.Update(flightOwner);
                await _context.SaveChangesAsync();

                // Step 3: Commit the transaction if all operations succeed
                await transaction.CommitAsync();

                return new GeneralResponse<object>(true, "Flight added successfully",flight.FlightId);
            }
            catch (Exception ex)
            {

                await transaction.RollbackAsync();
                return new GeneralResponse<object>(false, "Error while creating the flight");
            }
        }

        public async Task<GeneralResponse<object>> UpdateFlight(AddFlightDto updatedFlight, int AirlineId,int flightId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (updatedFlight == null) return new GeneralResponse<object>(false, "Model is invalid");

                var airline = await _context.Airlines.FirstOrDefaultAsync(a => a.AirlineId == AirlineId);
                if (airline == null) return new GeneralResponse<object>(false, "Airline is not found");

                var departAirport = await _context.Airports.FirstOrDefaultAsync(a => a.AirportId == updatedFlight.DepartAirport);
                var arrivalAirport = await _context.Airports.FirstOrDefaultAsync(a => a.AirportId == updatedFlight.ArrivalAirport);

                if (departAirport == null || arrivalAirport == null)
                {
                    return new GeneralResponse<object>(false, "Airport not found");
                }

                // Fetch the existing flight based on some unique identifier like FlightId or FlightNumber
                var existingFlight = await _context.Flights
                    .FirstOrDefaultAsync(f => f.FlightId==flightId);

                if (existingFlight == null) return new GeneralResponse<object>(false, "Flight not found");

                // Update flight properties
                existingFlight.FlightNumber = updatedFlight.FlightNumber;
                existingFlight.AirCraftType = updatedFlight.AirCraftType;
                existingFlight.FlightType = (TravelType)Enum.Parse(typeof(TravelType), updatedFlight.FlightType.ToUpper());
                existingFlight.DepartureAirportId = updatedFlight.DepartAirport;
                existingFlight.ArrivalAirportId = updatedFlight.ArrivalAirport;
                existingFlight.TotalSeats = updatedFlight.TotalSeats;
                existingFlight.TotalSeatColumn = updatedFlight.TotalSeatColumn;

                // Update flight entry in the database
                _context.Flights.Update(existingFlight);

                // Save changes after updating the flight details
                await _context.SaveChangesAsync();

                // Update flight owner if needed (e.g., increase TotalFlightsManaged if there is logic for it)
                var flightOwner = await _context.FlightOwners.FirstOrDefaultAsync(fo => fo.UserId == airline.FlightOwnerId);
                if (flightOwner == null)
                {
                    throw new Exception("Flight owner not found");
                }

                // Update any necessary flight owner properties (if required)
                _context.FlightOwners.Update(flightOwner);
                await _context.SaveChangesAsync();

                // Commit the transaction if all operations succeed
                await transaction.CommitAsync();

                return new GeneralResponse<object>(true, "Flight updated successfully", existingFlight.FlightId);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new GeneralResponse<object>(false, $"Error while updating the flight: {ex.Message}");
            }
        }

    }
}

