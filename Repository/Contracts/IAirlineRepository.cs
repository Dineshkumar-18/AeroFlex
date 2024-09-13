using AeroFlex.Dtos;
using AeroFlex.Models;

namespace AeroFlex.Repository.Contracts
{
    
        public interface IAirlineRepository
        {
            Task<IEnumerable<AirlineDto>> GetAllAirlinesAsync();
            Task<AirlineDto> GetAirlineByIdAsync(int id);
            Task<AirlineDto> CreateAirlineAsync(Airline airline);
            Task<AirlineDto> UpdateAirlineAsync(Airline airline);
            Task<bool> DeleteAirlineAsync(int id);
            Task<IEnumerable<AirlineDto>> GetAllAirlinesByFlightownerAsync(int? flightOwnerId);
    }
}
