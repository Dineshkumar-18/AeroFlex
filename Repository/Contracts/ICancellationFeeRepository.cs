using AeroFlex.Dtos;

namespace AeroFlex.Repository.Contracts
{
    public interface ICancellationFeeRepository
    {
        Task<IEnumerable<CancellationFeeDto>> GetAllAsync();
        Task<CancellationFeeDto> GetByIdAsync(int id);
        Task<CancellationFeeDto> CreateAsync(CancellationFeeDto dto);
        Task<CancellationFeeDto> UpdateAsync(int id, CancellationFeeDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
