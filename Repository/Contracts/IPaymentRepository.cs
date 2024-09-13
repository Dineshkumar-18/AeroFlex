using AeroFlex.Dtos;

namespace AeroFlex.Repository.Contracts
{
  
        public interface IPaymentRepository
        {
            Task<PaymentDto> ProcessPaymentAsync(PaymentDto paymentDto);
        }
}
