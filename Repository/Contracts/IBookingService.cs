using AeroFlex.Dtos;
using AeroFlex.Models;
using AeroFlex.Response;
using NuGet.Packaging.Signing;

namespace AeroFlex.Repository.Contracts
{
    public interface IBookingService
    {
        Task<GeneralResponse> CreateBookingAsync(BookingDto bookingDTO, int UserId);
        Task<Booking> GetBookingByIdAsync(int bookingId);
        Task<List<Booking>> GetUserBookingsAsync(int userId);

    }
}
