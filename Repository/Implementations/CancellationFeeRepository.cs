using AeroFlex.Data;
using AeroFlex.Dtos;
using AeroFlex.Models;
using AeroFlex.Repository.Contracts;
using Microsoft.EntityFrameworkCore;

namespace AeroFlex.Repository.Implementations
{
    public class CancellationFeeRepository : ICancellationFeeRepository
    {
        private readonly ApplicationDbContext _context;

        public CancellationFeeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CancellationFeeDto>> GetAllAsync()
        {
            var cancellationFees = await _context.CancellationFees
                .Include(f => f.FlightSchedule)
                .ToListAsync();

            return cancellationFees.Select(MapToDto).ToList();
        }

        public async Task<CancellationFeeDto> GetByIdAsync(int id)
        {
            var cancellationFee = await _context.CancellationFees
                .Include(f => f.FlightSchedule)
                .FirstOrDefaultAsync(c => c.CancellationFeeId == id);

            if (cancellationFee == null)
            {
                return null;
            }

            return MapToDto(cancellationFee);
        }

        public async Task<CancellationFeeDto> CreateAsync(CancellationFeeDto dto)
        {
            var flightSchedule = await _context.FlightsSchedules
                .FirstOrDefaultAsync(f => f.FlightScheduleId == dto.FlightScheduleId);

            if (flightSchedule == null)
            {
                throw new Exception("Flight schedule not found.");
            }

            var cancellationFee = new CancellationFee
            {
                FlightScheduleId = dto.FlightScheduleId,
                ChargeRate = dto.ChargeRate,
                PlatformFee = dto.PlatformFee,
                ApplicableDueDate = dto.ApplicableDueDate,
                FlightSchedule = flightSchedule
            };

            _context.CancellationFees.Add(cancellationFee);
            await _context.SaveChangesAsync();
            return MapToDto(cancellationFee);
        }

        public async Task<CancellationFeeDto> UpdateAsync(int id, CancellationFeeDto dto)
        {
            var cancellationFee = await _context.CancellationFees
                .FirstOrDefaultAsync(c => c.CancellationFeeId == id);

            if (cancellationFee == null)
            {
                throw new Exception("Cancellation fee not found.");
            }

            cancellationFee.FlightScheduleId = dto.FlightScheduleId;
            cancellationFee.ChargeRate = dto.ChargeRate;
            cancellationFee.PlatformFee = dto.PlatformFee;
            cancellationFee.ApplicableDueDate = dto.ApplicableDueDate;

            await _context.SaveChangesAsync();
            return MapToDto(cancellationFee);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var cancellationFee = await _context.CancellationFees
                .FirstOrDefaultAsync(c => c.CancellationFeeId == id);

            if (cancellationFee == null)
            {
                return false;
            }

            _context.CancellationFees.Remove(cancellationFee);
            await _context.SaveChangesAsync();
            return true;
        }


        private CancellationFeeDto MapToDto(CancellationFee cancellationFee)
        {
            return new CancellationFeeDto
            {
                CancellationFeeId = cancellationFee.CancellationFeeId,
                FlightScheduleId = cancellationFee.FlightScheduleId,
                ChargeRate = cancellationFee.ChargeRate,
                PlatformFee = cancellationFee.PlatformFee,
                ApplicableDueDate = cancellationFee.ApplicableDueDate
            };
        }

    }

}
