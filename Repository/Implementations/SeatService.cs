using AeroFlex.Controllers;
using AeroFlex.Data;
using AeroFlex.Dtos;
using AeroFlex.Models;
using AeroFlex.Repository.Contracts;
using AeroFlex.Response;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace AeroFlex.Repository.Implementations
{
    public class SeatService(ApplicationDbContext context) : ISeatService
    {


        public List<string> ParseLayoutPattern(string LayoutPattern)
        {
            // Initialize a list to hold the seat descriptions
            var seatDescriptions = new List<string>();

            // Split the layout pattern by '+' to differentiate between different seat blocks
            var blocks = LayoutPattern.Split('+');

            foreach (var block in blocks)
            {
                foreach (var seat in block)
                {
                    // Convert each seat character to its corresponding description
                    string seatDescription = GetSeatDescription(seat);
                    seatDescriptions.Add(seatDescription);
                }
            }

            return seatDescriptions;
        }

        private string GetSeatDescription(char seat)
        {
            return seat switch
            {
                'W' => "Window",
                'M' => "Middle",
                'A' => "Aisle",
                _ => "Unknown" // Handle any unknown seat types gracefully
            };
        }

        public async Task<GeneralResponse> AddSeatPricingWithPattern(SeatDto seatDto, int flightScheduleId)
        {

            var flightPricings = await context.FlightsPricings.FirstOrDefaultAsync(fp => fp.FlightScheduleId == flightScheduleId);
            if (flightPricings == null) return new GeneralResponse(false, "Flight Pricing Not found");

            var fs= await context.FlightsSchedules.FirstOrDefaultAsync(fs=>fs.FlightScheduleId==flightScheduleId);
            if (fs == null) return new GeneralResponse(false, "flightschedule not found");
            var seatLayout = await context.SeatLayouts.FirstOrDefaultAsync(sl => sl.FlightId == fs.FlightId);
            if (seatLayout == null) return new GeneralResponse(false, "seat layout not found");

            var seatpatternEncode = seatLayout.SeatTypePattern;
            var seatPatternDecoded=ParseLayoutPattern(seatpatternEncode);

            var totalColumns = await (from fli in context.Flights
                                      join fss in context.FlightsSchedules
                                      on fli.FlightId equals fss.FlightId
                                      where fss.FlightScheduleId == flightScheduleId
                                      select fli.TotalSeatColumn
                             ).FirstOrDefaultAsync();

            if (seatPatternDecoded.Count != totalColumns) return new GeneralResponse(false, "Seat type pattern is not match withe total number of columns in the flight");

            var disabledSeats = await context.UnavailableSeats
      .Where(s => s.FlightId == fs.FlightId)
      .Select(s => s.SeatNumber).ToListAsync();


            int currentStartRow = 1;
            foreach (var Class in seatDto.ClassNames)
            {
                var classInfo = await context.Classes.FirstOrDefaultAsync(c => c.ClassName == Class);
                if (classInfo == null) return new GeneralResponse(false, $"Class {Class} not found");

                // Find FlightScheduleClass to get the total seats for the class in the schedule
                var flightScheduleClass = await context.FlightScheduleClasses.FirstOrDefaultAsync(fsc => fsc.ClassId == classInfo.ClassId && fsc.FlightScheduleId == flightScheduleId);
                if (flightScheduleClass == null) return new GeneralResponse(false, $"Class {Class} not found in flight schedule");


                var seatlayout = await context.SeatLayouts.FirstOrDefaultAsync(sl => sl.FlightId == fs.FlightId && sl.ClassType == (Class == "Premium Economy" ? "Premium" : Class));

                int rowCount=seatLayout.RowCount;




                // Call a method to add seats for this specific class
                var response = await AddSeatsWithDynamicColumns(Class, seatPatternDecoded, flightScheduleId, rowCount, currentStartRow, totalColumns, flightScheduleClass, flightPricings,disabledSeats);
                currentStartRow += rowCount;

                if (!response.flag) return response;  // If any failure, return the error

            }
            return new GeneralResponse(true, "Seats added successfully for all classes.");
        }

        //dynamic column-seats

        private async Task<GeneralResponse> AddSeatsWithDynamicColumns(string Class, List<string> SeatTypePattern, int flightScheduleId,
                                                                      int rowCount, int currentStartRow, int totalColumns, FlightScheduleClass flightScheduleClass, FlightPricing flightPricings, List<String> disbledSeats)
        {
            var seatsToAdd = new List<Seat>();

            int currentRow = currentStartRow;

            for (int row = currentRow; row < currentRow+rowCount; row++)
            {
                for (char col = 'A'; col < 'A' + totalColumns; col++)
                {
                    string seatNumber = $"{row}{col}";

                    string seatType = SeatTypePattern[col-'A'];

                    if (!Enum.TryParse<SeatType>(seatType, true, out var seatTypeEnum))
                    {
                        return new GeneralResponse(false, "Invalid SeatType seat type should be in [Window,Aisle,Middle]");
                    }


                    var seatTypePricing = await context.SeatTypePricings.FirstOrDefaultAsync(stp => stp.FlightScheduleClassId == flightScheduleClass.FlightclassId && stp.SeatTypeName == seatTypeEnum);
                      if (seatTypePricing == null) return new GeneralResponse(false, $"Seat type pricing not found for class {Class}-{seatType}");

                    decimal ClassViseTaxAmount = flightScheduleClass.TotalPrice - flightScheduleClass.BasePrice;


                    if (!disbledSeats.Contains(seatNumber)) // Skip disabled seats
                    {
                        seatsToAdd.Add(new Seat
                        {
                            SeatNumber = seatNumber,
                            FlightScheduleId = flightScheduleId,
                            FlightScheduleClassId = flightScheduleClass.FlightclassId,
                            SeatTypePricingId = seatTypePricing.SeatTypePricingId,
                            TaxAmount = ClassViseTaxAmount + flightPricings.TaxAmount,
                            SeatPrice = seatTypePricing.TotalPriceByClassAndType + flightPricings.Totalprice
                        });
                    }
                }
            }

            if (seatsToAdd.Any())
            {
                await context.Seats.AddRangeAsync(seatsToAdd);
                await context.SaveChangesAsync();
            }

            return new GeneralResponse(true, "Seats added successfully.");
        }
        //{

        //    // Track the last seat number
        //    var existingSeats = await context.Seats
        //                            .Where(s => s.FlightScheduleId == flightScheduleId)
        //                            .OrderByDescending(s => s.SeatNumber)
        //                            .FirstOrDefaultAsync();

        //    int startRow = 1;
        //    char startColumn = 'A';

        //    // If seats exist, find the last seat's row and column
        //    if (existingSeats != null)
        //    {
        //        startRow = int.Parse(existingSeats.SeatNumber[..^1]);  // Extract row
        //        startColumn = existingSeats.SeatNumber[^1];             // Extract column

        //        // Move to the next column or row if needed
        //        if (startColumn == (char)('A' + totalColumns - 1))
        //        {
        //            // If column exceeds total columns, move to next row
        //            startRow++;
        //            startColumn = 'A';
        //        }
        //        else
        //        {
        //            // Otherwise, move to the next column
        //            startColumn++;
        //        }
        //    }

        //    int seatCount = 0;
        //    for (int row = startRow; seatCount < totalSeatsForClass; row++)
        //    {
        //        for (int col = (row == startRow ? startColumn - 'A' + 1 : 1); col <= totalColumns; col++)
        //        {
        //            string seatLabel = GetSeatLabel(col);

        //            // Determine seat type from SeatTypePattern using column index
        //            int patternIndex = (col - 1) % SeatTypePattern.Count;
        //            string seatType = SeatTypePattern[patternIndex];

        //            if(!Enum.TryParse<SeatType>(seatType,true,out var seatTypeEnum))
        //            {
        //                return new GeneralResponse(false, "Invalid SeatType seat type should be in [Window,Aisle,Middle]");
        //            }

        //            var seatTypePricing = await context.SeatTypePricings.FirstOrDefaultAsync(stp => stp.FlightScheduleClassId == flightScheduleClass.FlightclassId && stp.SeatTypeName== seatTypeEnum);
        //            if (seatTypePricing == null) return new GeneralResponse(false, $"Seat type pricing not found for class {Class}-{seatType}");

        //            decimal ClassViseTaxAmount = flightScheduleClass.TotalPrice - flightScheduleClass.BasePrice;

        //            var seat = new Seat
        //            {
        //                SeatNumber = $"{row}{seatLabel}",
        //                FlightScheduleId = flightScheduleId,
        //                FlightScheduleClassId = flightScheduleClass.FlightclassId,
        //                SeatTypePricingId = seatTypePricing.SeatTypePricingId,
        //                TaxAmount= ClassViseTaxAmount+flightPricings.TaxAmount,
        //                SeatPrice = seatTypePricing.TotalPriceByClassAndType + flightPricings.Totalprice
        //            };

        //            context.Seats.Add(seat);
        //            seatCount++;

        //            // If the total seat count is reached, break the loop
        //            if (seatCount >= flightScheduleClass.TotalSeats) break;
        //        }

        //        // Reset the column to start at A for the next row
        //        startColumn = 'A';

        //        // If the total seat count is reached, break the loop
        //        if (seatCount >= flightScheduleClass.TotalSeats) break;
        //    }

        //    await context.SaveChangesAsync();
        //    return new GeneralResponse(true, $"Seats added for class {Class}.");
        //}

        private string GetSeatLabel(int column)
        {
            // Generate the seat label based on column (e.g., A, B, C, D, etc.)
            return ((char)('A' + column-1)).ToString(); // Convert column number to letter
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

                var totalPrice = classPricingDto.BasePrice + ((classPricingDto.BasePrice * FlightTax.TaxRate)/100);

                var AddFlightScheduleClass = new FlightScheduleClass
                {
                    FlightScheduleId = FlightScheduleId,
                    ClassId = flightClass.ClassId,
                    BasePrice = classPricingDto.BasePrice,
                    TotalPrice = totalPrice,
                    FlightTaxId=FlightTax.FlightTaxId
                };

                context.Add(AddFlightScheduleClass);
            }
            await context.SaveChangesAsync();

            return new GeneralResponse(true, "FlightSchedule Class added successfully");
        }

        public async Task<GeneralResponse> SetSeatTypePricing(Dictionary<string, List<decimal>> seatTypePriceDtos, int FlightScheduleId)
        {
            if (seatTypePriceDtos == null || seatTypePriceDtos.Count == 0)
                return new GeneralResponse(false, "Model is invalid");
            
            FlightScheduleClass? FlightScheduleClass = null;

            var seatTypes = new List<string> { "WINDOW", "MIDDLE", "AISLE" };

            foreach (var entry in seatTypePriceDtos)
            {
                var className = entry.Key; 
                var seatPrices = entry.Value; 

                if (seatPrices == null || seatPrices.Count != seatTypes.Count)
                    return new GeneralResponse(false, "Invalid number of seat prices provided");

                // Retrieve the class from the database
                var Class = await context.Classes.FirstOrDefaultAsync(c => c.ClassName == className);
                if (Class == null)
                    return new GeneralResponse(false, $"Specified class {className} not found");

                // Retrieve the flight schedule class
                var flightScheduleClass = await context.FlightScheduleClasses.FirstOrDefaultAsync(fsc => fsc.FlightScheduleId == FlightScheduleId && fsc.ClassId == Class.ClassId);
                if (flightScheduleClass == null)
                    return new GeneralResponse(false, $"Class {className} not found in this flight schedule");

                // Loop through seat types and prices, and map them accordingly
                for (int i = 0; i < seatTypes.Count; i++)
                {
                    var seatType = seatTypes[i];
                    var seatPrice = seatPrices[i];

                    // Create a new SeatTypePricing entry
                    var seatTypePricing = new SeatTypePricing
                    {
                        FlightScheduleId = FlightScheduleId,
                        FlightScheduleClassId = flightScheduleClass.FlightclassId,
                        SeatTypeName = (SeatType)Enum.Parse(typeof(SeatType), seatType),
                        SeatPriceByType = seatPrice,
                        TotalPriceByClassAndType = flightScheduleClass.TotalPrice + seatPrice
                    };

                    context.SeatTypePricings.Add(seatTypePricing);
                }
            }

            await context.SaveChangesAsync();

            return new GeneralResponse(true,"SeatTypePricing is set successfully");
        }
    }
}
