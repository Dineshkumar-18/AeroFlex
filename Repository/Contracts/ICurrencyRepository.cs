using AeroFlex.Dtos;

namespace AeroFlex.Repository.Contracts
{
    public interface ICurrencyRepository
    {
        Task<IEnumerable<CurrencyDto>> GetAllCurrenciesAsync();
        Task<CurrencyDto> GetCurrencyByIdAsync(int id);
        Task<CurrencyDto> GetCurrencyByCountryAsync(string countryName);
        Task<CurrencyDto> CreateCurrencyAsync(CurrencyDto currencyDto);
        Task<CurrencyDto> UpdateCurrencyAsync(int id, CurrencyDto currencyDto);
        Task<bool> DeleteCurrencyAsync(int id);
    }

}
