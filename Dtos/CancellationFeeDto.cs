namespace AeroFlex.Dtos
{
    public class CancellationFeeDto
    {
        public int FlightScheduleId { get; set; }
        public decimal ChargeRate { get; set; }
        public decimal PlatformFee { get; set; }
    }   
}
