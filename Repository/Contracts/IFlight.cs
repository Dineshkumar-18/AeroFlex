using AeroFlex.Dtos;
using AeroFlex.Models;
using AeroFlex.Response;

namespace AeroFlex.Repository.Contracts
{
    public interface IFlight
    {
        Task<GeneralResponse> AddFlightSchedule(AddFlightScheduleDto FlgihtSchedule,int flightId);
        Task<GeneralResponse<object>> AddFlight(AddFlightDto addFlight, int AirlineId);

        Task<GeneralResponse<object>> UpdateFlight(AddFlightDto updatedFlight, int AirlineId, int flightId);

    }
}
