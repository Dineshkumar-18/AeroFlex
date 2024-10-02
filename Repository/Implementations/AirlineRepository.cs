using AeroFlex.Data;
using AeroFlex.Dtos;
using AeroFlex.Models;
using AeroFlex.Repository.Contracts;
using Microsoft.EntityFrameworkCore;

namespace AeroFlex.Repository.Implementations
{
    public class AirlineRepository : IAirlineRepository
{
    private readonly ApplicationDbContext _context;

    public AirlineRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<AirlineDto>> GetAllAirlinesAsync()
    {
        return await _context.Airlines
            .Select(a => new AirlineDto
            {
                AirlineId=a.AirlineId,
                AirlineName = a.AirlineName,
                IataCode = a.IataCode,
                Headquarters = a.Headquarters,
                Country = a.Country
            }).ToListAsync();
    }

    public async Task<Airline> GetAirlineByIdAsync(int id)
    {
        var airline = await _context.Airlines.FirstOrDefaultAsync(a => a.AirlineId == id);

        if (airline == null)
        {
            return null;
        }

            return airline;
    }

    public async Task<AirlineDto> CreateAirlineAsync(Airline airline)
    {
        await _context.Airlines.AddAsync(airline);
        await _context.SaveChangesAsync();

        return new AirlineDto
        {
            AirlineId=airline.AirlineId,
            AirlineName = airline.AirlineName,
            IataCode = airline.IataCode,
            Headquarters = airline.Headquarters,
            Country = airline.Country
        };
    }

    public async Task<AirlineDto> UpdateAirlineAsync(Airline airline)
    {
        _context.Airlines.Update(airline);
        await _context.SaveChangesAsync();

        return new AirlineDto
        {
            AirlineId=airline.AirlineId,
            AirlineName = airline.AirlineName,
            IataCode = airline.IataCode,
            Headquarters = airline.Headquarters,
            Country = airline.Country
        };
    }

    public async Task<bool> DeleteAirlineAsync(int id)
    {
        var airline = await _context.Airlines.FindAsync(id);
        if (airline == null)
        {
            return false;
        }

        _context.Airlines.Remove(airline);
        await _context.SaveChangesAsync();
        return true;
    }

        public async Task<IEnumerable<AirlineDto>> GetAllAirlinesByFlightownerAsync(int? flightOwnerId)
        {
           
            return await _context.Airlines.Where(a=>a.FlightOwnerId==flightOwnerId)
            .Select(a => new AirlineDto
            {
                AirlineId=a.AirlineId,
                AirlineName = a.AirlineName,
                IataCode = a.IataCode,
                Headquarters = a.Headquarters,
                Country = a.Country
            }).ToListAsync();
        }

    }

}
