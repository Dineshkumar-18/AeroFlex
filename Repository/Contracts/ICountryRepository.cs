using AeroFlex.Dtos;
using AeroFlex.Models;

namespace AeroFlex.Repository.Contracts
{
    public interface ICountryRepository
    {
        Task<Country> GetCountryByNameAsync(string countryName);
        Task<IEnumerable<CountryDto>> GetAllCountriesAsync();
        Task<CountryDto> GetCountryByIdAsync(int id);
        Task<CountryDto> CreateCountryAsync(CountryDto countryDto);
        Task<CountryDto> UpdateCountryAsync(int id, CountryDto countryDto);
        Task<bool> DeleteCountryAsync(int id);
    }

}
