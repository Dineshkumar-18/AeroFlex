using AeroFlex.Dtos;
using AeroFlex.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace AeroFlex.Repository.Contracts
{
    public interface IFlightScheduleRepository
    {
        Task<List<FlightScheduleDTO>> GetFlightSchedulesAsync(int departureAirport, int arrivalAirport, DateOnly date,string Class,int TotalPassengers);
    }
}
