using AeroFlex.Dtos;
using AeroFlex.Response;

namespace AeroFlex.Repository.Contracts
{
    public interface ISeatService
    {
        Task<GeneralResponse> AddSeatPricing(SeatDto seatDto,int FlightScheduleId);
        Task<GeneralResponse> SetClassPricing(ClassPricingDto classPricingDto,int FlightScheduleId);

        Task<GeneralResponse> SetSeatTypePricing(SeatTypePriceDto seatTypePriceDto,int FlightScheduleId);
    }
}
