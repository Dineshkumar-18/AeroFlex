using AeroFlex.Data;
using AeroFlex.Dtos;
using AeroFlex.Models;
using AeroFlex.Repository.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AeroFlex.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ApplicationDbContext _context;
        public PaymentController(IPaymentRepository paymentRepository,ApplicationDbContext context)
        {
            _paymentRepository = paymentRepository;
            _context = context;
        }

        // POST: api/Payment
        [HttpPost]
        public async Task<ActionResult<PaymentDto>> ProcessPayment(PaymentDto paymentDto)
        {
            try
            {
                var processedPayment = await _paymentRepository.ProcessPaymentAsync(paymentDto);
                return Ok(processedPayment);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]  
        [Route("paymentsuccess/{paymentId}")]
        public async Task<ActionResult> PaymentSuccessTicketGeneration(int paymentId)
        {
            var tickets=await _paymentRepository.PaymentSucessTicketGenerate(paymentId);
            if(!tickets.flag) return BadRequest("Error"+tickets.message);
            return Ok(tickets);
        }



        [HttpGet]
        [Authorize(Roles = "FlightOwner")]
        [Route("paymentinfo/{flightScheduleId}")]
        public async Task<ActionResult<PaymentDetailsByFlightSchduleDto>> PaymentInfoForParticularFlightSchedule(int flightScheduleId)
        {
            var flightOwnerId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (flightOwnerId == null) Unauthorized();

            var flightOwner = await (from flight in _context.Flights
                                     join fs in _context.FlightsSchedules
                                     on flight.FlightId equals fs.FlightId
                                     join airline in _context.Airlines
                                     on flight.AirlineId equals airline.AirlineId
                                     join fo in _context.FlightOwners
                                     on airline.FlightOwnerId equals fo.UserId
                                     where fs.FlightScheduleId == flightScheduleId
                                     select fo
                                     ).SingleOrDefaultAsync();
            if (flightOwner == null) return BadRequest("flight owner not found");
            if (flightOwner.UserId !=int.Parse(flightOwnerId)) return Forbid();

            var paymentInfo=await _paymentRepository.GetPaymentInfo(flightScheduleId);
            if(!paymentInfo.flag) return BadRequest(paymentInfo.message);
            return Ok(paymentInfo);

        }
    }

}
