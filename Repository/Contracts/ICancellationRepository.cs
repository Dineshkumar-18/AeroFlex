using AeroFlex.Dtos;
using AeroFlex.Response;

namespace AeroFlex.Repository.Contracts
{
    public interface ICancellationRepository
    {
        Task<GeneralResponse> CancellationProcess(Cancel Cancellation,int UserId,int FlightScheduleId);
        Task<CancellationDto> ViewCancellationDetails(int flightScheduleId);
        Task<CancellationDto> CancellationHistoryByUser(int UserId);
    }
}
