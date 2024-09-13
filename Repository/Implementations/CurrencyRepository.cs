using AeroFlex.Data;
using AeroFlex.Dtos;
using AeroFlex.Models;
using AeroFlex.Repository.Contracts;
using Microsoft.EntityFrameworkCore;

namespace AeroFlex.Repository.Implementations
{
    public class CurrencyRepository : ICurrencyRepository
    {
        private readonly ApplicationDbContext _context;

        public CurrencyRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CurrencyDto>> GetAllCurrenciesAsync()
        {
            return await _context.Currencies
                .Select(c => new CurrencyDto
                {
                    CurrencyCode = c.CurrencyCode,
                    Symbol = c.Symbol,
                    CurrencyName = c.CurrencyName
                }).ToListAsync();
        }

        public async Task<CurrencyDto> GetCurrencyByIdAsync(int id)
        {
            var currency = await _context.Currencies.FindAsync(id);
            if (currency == null)
            {
                return null;
            }

            return new CurrencyDto
            {
                CurrencyCode = currency.CurrencyCode,
                Symbol = currency.Symbol,
                CurrencyName = currency.CurrencyName
            };
        }

        public async Task<CurrencyDto> GetCurrencyByCountryAsync(string countryName)
        {
            var currency = await _context.Countries
                .Include(c => c.Currency)
                .Where(c => c.CountryName == countryName)
                .Select(c => c.Currency)
                .FirstOrDefaultAsync();

            if (currency == null)
            {
                return null;
            }

            return new CurrencyDto
            {
                CurrencyCode = currency.CurrencyCode,
                Symbol = currency.Symbol,
                CurrencyName = currency.CurrencyName
            };
        }

        public async Task<CurrencyDto> CreateCurrencyAsync(CurrencyDto currencyDto)
        {
            var currency = new Currency
            {
                CurrencyCode = currencyDto.CurrencyCode,
                Symbol = currencyDto.Symbol,
                CurrencyName = currencyDto.CurrencyName
            };

            await _context.Currencies.AddAsync(currency);
            await _context.SaveChangesAsync();

            return currencyDto;
        }

        public async Task<CurrencyDto> UpdateCurrencyAsync(int id, CurrencyDto currencyDto)
        {
            var currency = await _context.Currencies.FindAsync(id);
            if (currency == null)
            {
                return null;
            }

            currency.CurrencyCode = currencyDto.CurrencyCode;
            currency.Symbol = currencyDto.Symbol;
            currency.CurrencyName = currencyDto.CurrencyName;

            _context.Currencies.Update(currency);
            await _context.SaveChangesAsync();

            return currencyDto;
        }

        public async Task<bool> DeleteCurrencyAsync(int id)
        {
            var currency = await _context.Currencies.FindAsync(id);
            if (currency == null)
            {
                return false;
            }

            _context.Currencies.Remove(currency);
            await _context.SaveChangesAsync();

            return true;
        }
    }

}
