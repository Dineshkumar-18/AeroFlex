using AeroFlex.Dtos;
using AeroFlex.Models;
using AeroFlex.Response;

namespace AeroFlex.Repository.Contracts
{
    public interface IFlight
    {
        Task<GeneralResponse> AddFlightSchedule(FlightScheduleDTO FlgihtSchedule,int flightId);
        Task<GeneralResponse> AddFlight(AddFlightDto addFlight,int airlineId);
    }
}
