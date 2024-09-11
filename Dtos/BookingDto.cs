namespace AeroFlex.Dtos
{
    public class BookingDto
    {
        public int FlightSchedleId { get; set; }
        public DateTime BookindDate { get; set; }
        public List<Dictionary<string,PassengerDto>> SeatAllocations { get; set; }
    }
}
