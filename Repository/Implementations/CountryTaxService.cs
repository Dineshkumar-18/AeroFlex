using AeroFlex.Data;
using AeroFlex.Dtos;
using AeroFlex.Models;
using AeroFlex.Repository.Contracts;
using AeroFlex.Response;
using Microsoft.EntityFrameworkCore;

namespace AeroFlex.Repository.Implementations
{
    public class CountryTaxService : ICountryTaxService
    {
        private readonly ApplicationDbContext _context;

        public CountryTaxService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<GeneralResponse<IEnumerable<CountryTaxDto>>> GetAllAsync()
        {
            var countryTaxes = await _context.CountryTaxes.ToListAsync();
            var countryTaxDtos = countryTaxes.Select(ct => new CountryTaxDto
            {
                CountryId = ct.CountryId,
                TravelType = ct.TravelType.ToString(),
                Rate = ct.Rate
            });
            return new GeneralResponse<IEnumerable<CountryTaxDto>>(true, "Country taxes retrieved successfully", countryTaxDtos);
        }

        public async Task<GeneralResponse<CountryTaxDto>> GetByIdAsync(int id)
        {
            var countryTax = await _context.CountryTaxes.FindAsync(id);
            if (countryTax == null)
            {
                return new GeneralResponse<CountryTaxDto>(false, "Country tax not found");
            }

            var countryTaxDto = new CountryTaxDto
            {
                CountryId = countryTax.CountryId,
                TravelType = countryTax.TravelType.ToString(),
                Rate = countryTax.Rate
            };
            return new GeneralResponse<CountryTaxDto>(true, "Country tax retrieved successfully", countryTaxDto);
        }

        public async Task<GeneralResponse<string>> AddAsync(CountryTaxDto countryTaxDto)
        {

            if (!Enum.TryParse<TravelType>(countryTaxDto.TravelType, true, out var travelType))
            {
                return new GeneralResponse<string>(false, "Invalid TravelType", null);
            }
            var countryTax = new CountryTax
            {
                CountryId = countryTaxDto.CountryId,
                TravelType = travelType,
                Rate = countryTaxDto.Rate
            };
            await _context.CountryTaxes.AddAsync(countryTax);
            await _context.SaveChangesAsync();
            return new GeneralResponse<string>(true, "Country tax added successfully");
        }

        public async Task<GeneralResponse<string>> UpdateAsync(int id, CountryTaxDto countryTaxDto)
        {
            var existingCountryTax = await _context.CountryTaxes.FindAsync(id);
            if (existingCountryTax == null)
            {
                return new GeneralResponse<string>(false, "Country tax not found");
            }
            if (!Enum.TryParse<TravelType>(countryTaxDto.TravelType, true, out var travelType))
            {
                return new GeneralResponse<string>(false, "Invalid TravelType, TravelType should be in [Domestic,International]", null);
            }

            existingCountryTax.CountryId = countryTaxDto.CountryId;
            existingCountryTax.TravelType = travelType;
            existingCountryTax.Rate = countryTaxDto.Rate;

            _context.CountryTaxes.Update(existingCountryTax);
            await _context.SaveChangesAsync();
            return new GeneralResponse<string>(true, "Country tax updated successfully");
        }

        public async Task<GeneralResponse<string>> DeleteAsync(int id)
        {
            var existingCountryTax = await _context.CountryTaxes.FindAsync(id);
            if (existingCountryTax == null)
            {
                return new GeneralResponse<string>(false, "Country tax not found");
            }

            _context.CountryTaxes.Remove(existingCountryTax);
            await _context.SaveChangesAsync();
            return new GeneralResponse<string>(true, "Country tax deleted successfully");
        }
    }
}
