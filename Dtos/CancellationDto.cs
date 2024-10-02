namespace AeroFlex.Dtos
{
    public class CancellationDto
    {
        public int CancellationId { get; set; }
        public string SeatNumber { get; set; }
        public int PassengerId { get; set; }
        public int BookingId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal DeductionAmount { get; set; }
        public decimal RefundableAmount { get; set; }
        public bool IsRefundabale { get; set; }
        public DateTime CancellationAppliedDate { get; set; }
    }
}
