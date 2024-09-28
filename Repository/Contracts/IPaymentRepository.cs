using AeroFlex.Dtos;
using AeroFlex.Response;

namespace AeroFlex.Repository.Contracts
{
  
        public interface IPaymentRepository
        {
            Task<List<TicketDto>> ProcessPaymentAsync(PaymentDto paymentDto);
            Task<GeneralResponse<PaymentDetailsByFlightSchduleDto>> GetPaymentInfo(int flightscheduleId);
            Task<GeneralResponse<List<TicketDto>>> PaymentSucessTicketGenerate(int paymentId);
    }
}
