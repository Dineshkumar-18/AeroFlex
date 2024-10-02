namespace AeroFlex.Dtos
{
    public class UnavailableSeatsRequest
    {
        public int FlightId { get; set; }
        public Dictionary<string, List<string>> Seats { get; set; } = new Dictionary<string, List<string>>();
    }
}
