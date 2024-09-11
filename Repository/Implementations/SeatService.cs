using AeroFlex.Controllers;
using AeroFlex.Data;
using AeroFlex.Dtos;
using AeroFlex.Models;
using AeroFlex.Repository.Contracts;
using AeroFlex.Response;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using SimplyFly.Models;

namespace AeroFlex.Repository.Implementations
{
    public class SeatService(ApplicationDbContext context) : ISeatService
    {
        public async Task<GeneralResponse> AddSeatPricing(List<SeatDto> seatDtos,int flightScheduleId)
        {
            //Retrieving the classes
            var totalColumns = await (from fli in context.Flights
                              join fs in context.FlightsSchedules
                              on fli.FlightId equals fs.FlightId
                              where fs.FlightScheduleId==flightScheduleId
                              select fli.TotalSeatColumn
                              ).FirstOrDefaultAsync();

           

            foreach (var seatDto in seatDtos)
            {
                // Retrieve the class information based on the ClassName
                var classInfo = await context.Classes.FirstOrDefaultAsync(c => c.ClassName == seatDto.ClassName);
                if (classInfo == null) return new GeneralResponse(false, $"Class {seatDto.ClassName} not found");

                // Find FlightScheduleClass to get the total seats for the class in the schedule
                var flightScheduleClass = await context.FlightScheduleClasses.FirstOrDefaultAsync(fsc => fsc.ClassId == classInfo.ClassId && fsc.FlightScheduleId == flightScheduleId);
                if (flightScheduleClass == null) return new GeneralResponse(false, $"Class {seatDto.ClassName} not found in flight schedule");

                int totalSeatsForClass = flightScheduleClass.TotalSeats;
                // Calculate number of rows
                int totalRows = (int)Math.Ceiling((double)totalSeatsForClass / totalColumns); 

                // Call a method to add seats for this specific class
                var response = await AddSeatsWithDynamicColumns(seatDto, flightScheduleId, totalColumns, totalRows);
                if (!response.flag) return response;  // If any failure, return the error
            }

            return new GeneralResponse(true, "Seats added successfully for all classes.");
        }

        //dynamic column-seats

        public async Task<GeneralResponse> AddSeatsWithDynamicColumns(SeatDto seatDto, int flightScheduleId, int totalColumns, int totalRows)
        {
            var classInfo = await context.Classes.FirstOrDefaultAsync(c => c.ClassName == seatDto.ClassName);
            if (classInfo == null) return new GeneralResponse(false, $"Class {seatDto.ClassName} not found");

            var flightScheduleClass = await context.FlightScheduleClasses.FirstOrDefaultAsync(fsc => fsc.ClassId == classInfo.ClassId && fsc.FlightScheduleId == flightScheduleId);
            if (flightScheduleClass == null) return new GeneralResponse(false, $"Class {seatDto.ClassName} not found in flight schedule");

            var seatNumber = 1;
            for (int row = 1; row <= totalRows; row++)
            {
                for (int col = 1; col <= totalColumns; col++)
                {
                    string seatLabel = GetSeatLabel(col);

                    // determine the seat type based on column (window, aisle, middle)
                    var seatType = GetSeatTypeByColumn(col, totalColumns);

                    var seatTypePricing = await context.SeatTypePricings.FirstOrDefaultAsync(stp => stp.FlightScheduleClassId == flightScheduleClass.FlightclassId && stp.SeatTypeName == seatType);
                    if (seatTypePricing == null) return new GeneralResponse(false, $"Seat type pricing not found for class {seatDto.ClassName}");

                    var seat = new Seat
                    {
                        SeatNumber = $"{row}{seatLabel}",
                        FlightScheduleClassId = flightScheduleClass.FlightclassId,
                        SeatTypePricingId = seatTypePricing.SeatTypePricingId,
                        SeatPrice = seatTypePricing.TotalPriceByClassAndType
                    };

                    context.Seats.Add(seat);
                    seatNumber++;

                    // If the total seat count is reached, break the loop
                    if (seatNumber > flightScheduleClass.TotalSeats) break;
                }

                // Break if total seat count is reached
                if (seatNumber > flightScheduleClass.TotalSeats) break;
            }

            await context.SaveChangesAsync();
            return new GeneralResponse(true, $"Seats added for class {seatDto.ClassName}.");
        }


        private string GetSeatLabel(int column)
        {
            // Generate the seat label based on column (e.g., A, B, C, D, etc.)
            return ((char)(64 + column)).ToString(); // Convert column number to letter
        }

        private SeatType GetSeatTypeByColumn(int column, int totalColumns)
        {
            if (totalColumns == 5)
            {
                if (column == 1 || column == 5) return SeatType.WINDOW;
                if (column == 2 || column == 4) return SeatType.AISLE;
                return SeatType.MIDDLE;
            }
            else if (totalColumns == 6)
            {
                if (column == 1 || column == 6) return SeatType.WINDOW;
                if (column == 2 || column == 5) return SeatType.AISLE;
                return SeatType.MIDDLE;
            }
            else
            {
                // Handle other column configurations dynamically
                return column == 1 || column == totalColumns ? SeatType.WINDOW : column == 2 || column == totalColumns - 1 ? SeatType.AISLE : SeatType.MIDDLE;
            }
        }

        public async Task<GeneralResponse> SetClassPricing(List<ClassPricingDto> classPricingDtos, int FlightScheduleId)
        {

            if (classPricingDtos == null || !classPricingDtos.Any())
                return new GeneralResponse(false, "Class pricing data is invalid or empty");

             var flightSchedule = await context.FlightsSchedules
            .Include(fs => fs.Flight)
            .FirstOrDefaultAsync(fs => fs.FlightScheduleId == FlightScheduleId);

            if (flightSchedule == null)
                return new GeneralResponse(false, "Flight schedule not found");

            var AirportLocation = await (from airport in context.Airports
                                         join flightschedule in context.FlightsSchedules
                                         on airport.AirportId equals flightschedule.DepartureAirportId
                                         where flightschedule.FlightScheduleId == FlightScheduleId
                                         select new
                                         {
                                             CountryId=airport.CountryId,
                                             Traveltype=flightschedule.Flight.FlightType
                                         }
                               ).FirstOrDefaultAsync();

            if (AirportLocation == null)
                return new GeneralResponse(false, "Departure airport information not found");

            foreach (var classPricingDto in classPricingDtos)
            {


                var flightClass = await context.Classes.FirstOrDefaultAsync(c => c.ClassName == classPricingDto.ClassName);

                if (flightClass == null)
                    return new GeneralResponse(false, $"Class '{classPricingDto.ClassName}' not found");

                var FlightTax = await context.FlightTaxes.FirstOrDefaultAsync(ft => ft.CountryId == AirportLocation.CountryId && ft.ClassId == flightClass.ClassId && ft.TravelType == AirportLocation.Traveltype);

                if (FlightTax == null)
                    return new GeneralResponse(false, $"Tax information not found for class '{classPricingDto.ClassName}'");

                var totalPrice = classPricingDto.BasePrice + (classPricingDto.BasePrice * FlightTax.TaxRate);

                var AddFlightScheduleClass = new FlightScheduleClass
                {
                    FlightScheduleId = FlightScheduleId,
                    ClassId = flightClass.ClassId,
                    BasePrice = classPricingDto.BasePrice,
                    TotalPrice = totalPrice,
                    TotalSeats = classPricingDto.TotalSeats
                };

                context.Add(AddFlightScheduleClass);
            }
            await context.SaveChangesAsync();

            return new GeneralResponse(true, "FlightSchedule Class added successfully");
        }

        public async Task<GeneralResponse> SetSeatTypePricing(List<SeatTypePriceDto> seatTypePriceDtos, int FlightScheduleId)
        {
            if (seatTypePriceDtos == null || !seatTypePriceDtos.Any())
                return new GeneralResponse(false, "Model is invalid");

            for (int i = 0; i < seatTypePriceDtos.Count; i++)
            {
                var seatTypePriceDto = seatTypePriceDtos[i];

                FlightScheduleClass? FlightScheduleClass = null;

                if (i % 3 == 0)
                {
                    var className = seatTypePriceDto.ClassName;

                    var Class = await context.Classes.FirstOrDefaultAsync(c => c.ClassName == className);
                    if (Class == null) return new GeneralResponse(false, $"Specified class {className} not found");

                     FlightScheduleClass = await context.FlightScheduleClasses.FirstOrDefaultAsync(fsc => fsc.FlightScheduleId == FlightScheduleId && fsc.ClassId == Class.ClassId);

                    if (FlightScheduleClass == null) return new GeneralResponse(false, "Classname is not found in this flight schedule");
                }

                var Seattypepricing = new SeatTypePricing
                {
                    FlightScheduleId = FlightScheduleId,
                    FlightScheduleClassId = FlightScheduleClass.ClassId,
                    SeatTypeName = (SeatType)Enum.Parse(typeof(SeatType), seatTypePriceDto.SeatType.ToUpper()),
                    SeatPriceByType = seatTypePriceDto.SeatTypePrice,
                    TotalPriceByClassAndType = (FlightScheduleClass.TotalPrice + seatTypePriceDto.SeatTypePrice)
                };

                context.Add(Seattypepricing);
            }
            await context.SaveChangesAsync();

            return new GeneralResponse(true,"SeatTypePricing is set successfully");
        }
    }
}
