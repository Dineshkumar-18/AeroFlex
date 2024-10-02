using AeroFlex.Dtos;
using AeroFlex.Models;
using AeroFlex.Response;

namespace AeroFlex.Repository.Contracts
{
        
        public interface IFlightPricingService
        {
            Task<FlightPricingDto> GetFlightPricingAsync(int flightScheduleId);
            Task<GeneralResponse> SetFlightPricingAsync(FlightPricingDto flightPricingDto, FlightSchedule flightSchedule);
            Task<FlightPricingDto> UpdateFlightPricingAsync(FlightPricingDto flightPricingDto);
        }


}
