using AeroFlex.Dtos;
using AeroFlex.Response;
using System.Threading.Tasks;

namespace AeroFlex.Repository.Contracts
{
    public interface ISeatService
    {
        Task<GeneralResponse> AddSeatPricing(List<SeatDto> seatDto,int FlightScheduleId);
        Task<GeneralResponse> SetClassPricing(List<ClassPricingDto> classPricingDto,int FlightScheduleId);

        Task<GeneralResponse> SetSeatTypePricing(List<SeatTypePriceDto> seatTypePriceDtos, int FlightScheduleId);

        Task<GeneralResponse> AddSeatsWithDynamicColumns(SeatDto seatDto, int flightScheduleId, int totalColumns, int totalRows);
    }
}

