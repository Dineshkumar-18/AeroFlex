using AeroFlex.Dtos;
using AeroFlex.Models;
using AeroFlex.Response;
using Microsoft.AspNetCore.Mvc;

namespace AeroFlex.Repository.Contracts
{
    public interface ITicketRepository
    {
        Task<List<TicketDto>> GenerateTicket(int flightScheduleid,int bookingId);
        Task<GeneralResponse> UploadTickets(List<TicketDto> ticketDto,IFormFile[] tickets);
    }
}
