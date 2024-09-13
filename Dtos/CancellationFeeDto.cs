namespace AeroFlex.Dtos
{
    public class CancellationFeeDto
    {
        public int CancellationFeeId { get; set; }
        public int FlightScheduleId { get; set; }
        public decimal ChargeRate { get; set; }
        public decimal PlatformFee { get; set; }
        public DateTime ApplicableDueDate { get; set; }
    }


}
