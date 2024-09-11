using AeroFlex.Dtos;
using AeroFlex.Models;

namespace AeroFlex.Repository.Contracts
{
    
        public interface IFlightPricingService
        {
            Task<FlightPricingDto> GetFlightPricingAsync(int flightScheduleId);
            Task<FlightPricingDto> SetFlightPricingAsync(FlightPricingDto flightPricingDto, FlightSchedule flightSchedule);
            Task<FlightPricingDto> UpdateFlightPricingAsync(FlightPricingDto flightPricingDto);
        }
  
}
