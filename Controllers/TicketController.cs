using AeroFlex.Dtos;
using AeroFlex.Repository.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AeroFlex.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly ITicketRepository _ticketRepository;

        // Constructor
        public TicketController(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }

        [HttpGet]
        [Route("generateTicket")]
        public async Task<ActionResult> GenerateTicket([FromQuery] int flightScheduleId, [FromQuery] int bookingId)
        {
            var tickets = await _ticketRepository.GenerateTicket(flightScheduleId, bookingId);
            return Ok(tickets);
        }

        [HttpPost("send-ticket-email")]
        public async Task<IActionResult> SendTicketEmail([FromForm] string ticketInfo, [FromForm] IFormFile[] tickets)
        {
            var tick = JsonConvert.DeserializeObject<List<TicketDto>>(ticketInfo);
            if (tickets == null || !tickets.Any()) return BadRequest("Tickets not found");
            var emailResponse = await _ticketRepository.UploadTickets(tick,tickets);
            if (!emailResponse.flag) return BadRequest("error while sending the email");
            return Ok(emailResponse);

        }
    }
}
