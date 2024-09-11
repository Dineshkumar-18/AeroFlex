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
        public async Task<GeneralResponse> AddSeatPricing(SeatDto seatDto,int FlightScheduleId)
        {
            //Retrieving the classes
            var Class=await context.Classes.FirstOrDefaultAsync(c=>c.ClassName==seatDto.ClassName);

            if (Class == null) return new GeneralResponse(false, "Specified class not found");
            //for finding the class based pricing
            var FlightScheduleClass = await context.FlightScheduleClasses.FirstOrDefaultAsync(sfc=>sfc.ClassId==Class.ClassId && sfc.FlightScheduleId==FlightScheduleId);


            
            var Seattype = await context.SeatTypePricings.FirstOrDefaultAsync(stp=>stp.FlightScheduleClassId==Class.ClassId && stp.SeatTypeName.ToString()==seatDto.SeatType.ToUpper());

            var AddSeat = new Seat
            {
                SeatNumber = seatDto.SeatNumber,
                FlightScheduleClassId = FlightScheduleClass.FlightclassId,
                SeatTypePricingId = Seattype.SeatTypePricingId,
                SeatPrice=Seattype.TotalPriceByClassAndType,
            };

            context.Add(AddSeat);
            await context.SaveChangesAsync();

            return new GeneralResponse(true, "Seat added successfully");
        }

        public async Task<GeneralResponse> SetClassPricing(ClassPricingDto classPricingDto, int FlightScheduleId)
        {

            if (classPricingDto == null) return new GeneralResponse(false,"Model is invalid");

            var Class = await context.Classes.FirstOrDefaultAsync(c=>c.ClassName==classPricingDto.ClassName);

            if (Class == null) return new GeneralResponse(false, "Specified class not found");

            var AirportLocation = await (from airport in context.Airports
                                         join flightschedule in context.FlightsSchedules
                                         on airport.AirportId equals flightschedule.DepartureAirportId
                                         join flight in context.Flights
                                         on flightschedule.FlightId equals flight.FlightId
                                         where flightschedule.FlightScheduleId == FlightScheduleId
                                         select new
                                         {
                                             CountryId=airport.CountryId,
                                             FlightId=flightschedule.FlightId,
                                             Traveltype=flight.FlightType
                                         }
                               ).FirstOrDefaultAsync();

            var FlightTax = await context.FlightTaxes.FirstOrDefaultAsync(ft=>ft.CountryId==AirportLocation.CountryId && ft.ClassId==Class.ClassId && ft.TravelType== AirportLocation.Traveltype);

            var AddFlightScheduleClass = new FlightScheduleClass
            {
                FlightScheduleId = FlightScheduleId,
                ClassId = Class.ClassId,
                BasePrice = classPricingDto.BasePrice,
                TotalPrice = classPricingDto.BasePrice + (classPricingDto.BasePrice*FlightTax!.TaxRate),
                TotalSeats = classPricingDto.TotalSeats
            };

            context.Add(AddFlightScheduleClass);
            await context.SaveChangesAsync();

            return new GeneralResponse(true, "FlightSchedule Class added successfully");
        }

        public async Task<GeneralResponse> SetSeatTypePricing(SeatTypePriceDto seatTypePriceDto, int FlightScheduleId)
        {
            if(seatTypePriceDto == null) return new GeneralResponse(false,"Model is invalid");
            var Class = await context.Classes.FirstOrDefaultAsync(c => c.ClassName == seatTypePriceDto.ClassName);
            if (Class == null) return new GeneralResponse(false, "Specified class not found");

            var FlightScheduleClass = await context.FlightScheduleClasses.FirstOrDefaultAsync(fsc=>fsc.FlightScheduleId==FlightScheduleId && fsc.ClassId==Class.ClassId);

            if(FlightScheduleClass==null) return new GeneralResponse(false,"Classname is not found in this flight schedule");

            var Seattypepricing = new SeatTypePricing
            {
                FlightScheduleId = FlightScheduleId,
                FlightScheduleClassId=FlightScheduleClass.ClassId,
                SeatTypeName= (SeatType)Enum.Parse(typeof(SeatType), seatTypePriceDto.SeatType.ToUpper()),
                SeatPriceByType=seatTypePriceDto.SeatTypePrice,
                TotalPriceByClassAndType=(FlightScheduleClass.TotalPrice+ seatTypePriceDto.SeatTypePrice)
            };

            context.Add(Seattypepricing);
            await context.SaveChangesAsync();

            return new GeneralResponse(true,"SeatTypePricing is set successfully");
        }
    }
}
