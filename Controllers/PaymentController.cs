using AeroFlex.Dtos;
using AeroFlex.Repository.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AeroFlex.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentRepository _paymentRepository;

        public PaymentController(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
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
    }

}
