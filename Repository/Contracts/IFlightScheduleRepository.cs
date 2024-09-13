using AeroFlex.Dtos;
using AeroFlex.Models;

namespace AeroFlex.Repository.Contracts
{
    public interface IFlightScheduleRepository
    {
        Task<IEnumerable<FlightScheduleDTO>> GetFlightSchedulesAsync(string departureAirport, string arrivalAirport, DateTime date);
    }
}
