using AeroFlex.Data;
using AeroFlex.Dtos;
using AeroFlex.Models;
using AeroFlex.Repository.Contracts;
using AeroFlex.Response;
using Microsoft.EntityFrameworkCore;

namespace AeroFlex.Repository.Implementations
{
    public class FlightPricingService(ApplicationDbContext context) : IFlightPricingService
    {
        public Task<FlightPricingDto> GetFlightPricingAsync(int flightScheduleId)
        {
            throw new NotImplementedException();
        }

        public async Task<GeneralResponse> SetFlightPricingAsync(FlightPricingDto flightPricingDto, FlightSchedule flightSchedule)
        {

            var flight = await context.Flights.Where(f => f.FlightId == flightSchedule.FlightId).FirstOrDefaultAsync();
            var flightType = flight!.FlightType;

            var country = await (from airport in context.Airports
                                 join ctry in context.Countries
                                 on airport.CountryId equals ctry.CountryId
                                 where airport.AirportId == flightSchedule.DepartureAirportId
                                 select ctry
                               ).FirstOrDefaultAsync();

            if (country == null) return new GeneralResponse(false,"country not found");

            var Countrytax = await context.CountryTaxes.FirstOrDefaultAsync(ct => ct.CountryId == country!.CountryId && ct.TravelType == flightType);

            if (Countrytax == null) return new GeneralResponse(false,"Country tax not found");


            var flightPricing = new FlightPricing
            {
                FlightScheduleId = flightSchedule.FlightScheduleId,
                CountryTaxId=Countrytax!.CountryTaxId,
                BasePrice = flightPricingDto.BasePrice,
                DemandMultiplier = flightPricingDto.DemandMultiplier,
                SeasonalMultiplier = flightPricingDto.SeasonalMultiplier,
                Discount=flightPricingDto.Discount,
                Totalprice = CalculateTotalPrice(flightPricingDto)
            };

            context.FlightsPricings.Add(flightPricing);
            await context.SaveChangesAsync();

            return new GeneralResponse(true,"Flight pricing set successfully");

        }

        public Task<FlightPricingDto> UpdateFlightPricingAsync(FlightPricingDto flightPricingDto)
        {
            throw new NotImplementedException();
        }


        private decimal CalculateTotalPrice(FlightPricingDto model)
        {
            decimal basePrice = model.BasePrice;
            decimal seasonalMultiplier = model.SeasonalMultiplier ?? 1;
            decimal demandMultiplier = model.DemandMultiplier ?? 1;
            decimal discount = model.Discount ?? 0;

            return basePrice * seasonalMultiplier * demandMultiplier - discount;
        }
    }
}
