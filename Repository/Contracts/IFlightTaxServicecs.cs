using AeroFlex.Models;

namespace AeroFlex.Repository.Contracts
{
    public interface IFlightTaxRepository
    {
        Task<List<FlightTax>> GetExistingTaxesAsync(int countryId, List<int> classIds, string travelType);
        Task<bool> AddBulkFlightTaxesAsync(List<FlightTax> taxes);
    }
}

