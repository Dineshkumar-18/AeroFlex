using AeroFlex.Dtos;
using AeroFlex.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace AeroFlex.Repository.Contracts
{
    public interface IFlightScheduleRepository
    {
        Task<List<FlightScheduleDTO>> GetFlightSchedulesAsync(string departureAirport, string arrivalAirport, DateOnly date,string Class,int TotalPassengers);
    }
}
