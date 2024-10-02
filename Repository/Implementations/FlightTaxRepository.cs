using AeroFlex.Data;
using AeroFlex.Models;
using AeroFlex.Repository.Contracts;
using Microsoft.EntityFrameworkCore;

namespace AeroFlex.Repository.Implementations
{
    public class FlightTaxRepository : IFlightTaxRepository
    {
        private readonly ApplicationDbContext _context;

        public FlightTaxRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<FlightTax>> GetExistingTaxesAsync(int countryId, List<int> classIds, string travelType)
        {
            TravelType travelTypeEnum;

            if (Enum.TryParse(travelType, true, out travelTypeEnum))
            {
                return await _context.FlightTaxes
                    .Where(f => f.CountryId == countryId && classIds.Contains(f.ClassId) && f.TravelType == travelTypeEnum)
                    .ToListAsync();
            }
            return null;
        }

        public async Task<bool> AddBulkFlightTaxesAsync(List<FlightTax> taxes)
        {
            try
            {
                await _context.FlightTaxes.AddRangeAsync(taxes);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

}
