using AeroFlex.Dtos;
using AeroFlex.Response;
using System.Threading.Tasks;

namespace AeroFlex.Repository.Contracts
{
    public interface ISeatService
    {
        Task<GeneralResponse> AddSeatPricingWithPattern(SeatDto seatDto, int flightScheduleId);
        Task<GeneralResponse> SetClassPricing(List<ClassPricingDto> classPricingDto,int FlightScheduleId);

        Task<GeneralResponse> SetSeatTypePricing(Dictionary<string, List<decimal>> seatTypePriceDtos, int FlightScheduleId);
    }
}
    
