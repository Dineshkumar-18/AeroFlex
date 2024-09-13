namespace AeroFlex.Dtos
{
    public class BookingDto
    {
        public List<Dictionary<string,PassengerDto>> SeatAllocations { get; set; }
    }
}
