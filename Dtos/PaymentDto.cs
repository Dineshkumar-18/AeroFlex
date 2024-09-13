using AeroFlex.Models;

namespace AeroFlex.Dtos
{
    public class PaymentDto
    {
        public int BookingId { get; set; }
        public int ReferenceId { get; set; }
        public decimal PaidAmount { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public DateTime PaymentDate { get; set; }
    }

}
