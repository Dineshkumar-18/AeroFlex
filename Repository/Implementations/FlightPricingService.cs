using AeroFlex.Data;
using AeroFlex.Dtos;
using AeroFlex.Models;
using AeroFlex.Repository.Contracts;
using Microsoft.EntityFrameworkCore;

namespace AeroFlex.Repository.Implementations
{
    public class FlightPricingService(ApplicationDbContext context) : IFlightPricingService
    {
        public Task<FlightPricingDto> GetFlightPricingAsync(int flightScheduleId)
        {
            throw new NotImplementedException();
        }

        public async Task<FlightPricingDto> SetFlightPricingAsync(FlightPricingDto flightPricingDto, FlightSchedule flightSchedule)
        {

            var flight = await context.Flights.Where(f => f.FlightId == flightSchedule.FlightId).FirstOrDefaultAsync();
            var flightType = flight.FlightType;

            var country = await (from airport in context.Airports
                                 join ctry in context.Countries
                                 on airport.CountryId equals ctry.CountryId
                                 where airport.AirportId == flightSchedule.DepartureAirportId
                                 select ctry
                               ).FirstOrDefaultAsync();

            var CountryTax = await context.CountryTaxes.FirstOrDefaultAsync(ct => ct.CountryId == country!.CountryId && ct.TravelType == flightType);


            var flightPricing = new FlightPricing
            {
                FlightScheduleId = flightSchedule.FlightScheduleId,
                BasePrice = flightPricingDto.BasePrice,
                DemandMultiplier = flightPricingDto.DemandMultiplier,
                SeasonalMultiplier = flightPricingDto.SeasonalMultiplier,
                Totalprice = CalculateTotalPrice(flightSchedule)
            };


            throw new NotImplementedException();
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
