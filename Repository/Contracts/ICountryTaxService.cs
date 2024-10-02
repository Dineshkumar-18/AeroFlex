using AeroFlex.Dtos;
using AeroFlex.Response;

namespace AeroFlex.Repository.Contracts
{
    public interface ICountryTaxService
    {
        Task<GeneralResponse<IEnumerable<CountryTaxDto>>> GetAllAsync();
        Task<GeneralResponse<CountryTaxDto>> GetByIdAsync(int id);
        Task<GeneralResponse<string>> AddAsync(CountryTaxDto countryTaxDto);
        Task<GeneralResponse<string>> UpdateAsync(int id, CountryTaxDto countryTaxDto);
        Task<GeneralResponse<string>> DeleteAsync(int id);
    }
}
