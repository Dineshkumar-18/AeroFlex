using AeroFlex.Data;
using AeroFlex.Dtos;
using AeroFlex.Models;
using AeroFlex.Repository.Contracts;
using Microsoft.EntityFrameworkCore;

namespace AeroFlex.Repository.Implementations
{
    public class CountryRepository : ICountryRepository
    {
        private readonly ApplicationDbContext _context;

        public CountryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Country> GetCountryByNameAsync(string countryName)
        {
            return await _context.Countries
                .FirstOrDefaultAsync(c => c.CountryName.Equals(countryName));
        }
        public async Task<IEnumerable<CountryDto>> GetAllCountriesAsync()
        {
            return await _context.Countries
                .Select(c => new CountryDto
                {
                    CountryName = c.CountryName,
                    CountryCode = c.CountryCode,
                    CurrencyName = c.Currency.CurrencyName
                }).ToListAsync();
        }

        public async Task<CountryDto> GetCountryByIdAsync(int id)
        {
            var country = await _context.Countries.Include(c => c.Currency).FirstOrDefaultAsync(c => c.CountryId == id);

            if (country == null)
            {
                return null;
            }

            return new CountryDto
            {
                CountryName = country.CountryName,
                CountryCode = country.CountryCode,
                CurrencyName = country.Currency.CurrencyName
            };
        }

        public async Task<CountryDto> CreateCountryAsync(CountryDto countryDto)
        {
            // Fetch the Currency by CurrencyName
            var currency = await _context.Currencies.FirstOrDefaultAsync(c => c.CurrencyName == countryDto.CurrencyName);

            if (currency == null)
            {
                throw new ArgumentException("Invalid CurrencyName");
            }

            var country = new Country
            {
                CountryName = countryDto.CountryName,
                CountryCode = countryDto.CountryCode,
                CurrencyId = currency.CurrencyId
            };

            await _context.Countries.AddAsync(country);
            await _context.SaveChangesAsync();

            return new CountryDto
            {
                CountryName = country.CountryName,
                CountryCode = country.CountryCode,
                CurrencyName = currency.CurrencyName
            };
        }

        public async Task<CountryDto> UpdateCountryAsync(int id, CountryDto countryDto)
        {
            var country = await _context.Countries.FindAsync(id);
            if (country == null)
            {
                return null;
            }

            var currency = await _context.Currencies.FirstOrDefaultAsync(c => c.CurrencyName == countryDto.CurrencyName);
            if (currency == null)
            {
                throw new ArgumentException("Invalid CurrencyName");
            }

            country.CountryName = countryDto.CountryName;
            country.CountryCode = countryDto.CountryCode;
            country.CurrencyId = currency.CurrencyId;

            _context.Countries.Update(country);
            await _context.SaveChangesAsync();

            return new CountryDto
            {
                CountryName = country.CountryName,
                CountryCode = country.CountryCode,
                CurrencyName = currency.CurrencyName
            };
        }

        public async Task<bool> DeleteCountryAsync(int id)
        {
            var country = await _context.Countries.FindAsync(id);
            if (country == null)
            {
                return false;
            }

            _context.Countries.Remove(country);
            await _context.SaveChangesAsync();
            return true;
        }


    }
}
