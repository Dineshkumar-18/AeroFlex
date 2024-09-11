namespace AeroFlex.Dtos
{
    public class AssignFlightSegmentDTO
    {
        public int FlightSegmentId { get; set; }
        public int ItineraryId { get; set; }  // Include ItineraryId
        public int StopOrder { get; set; }
        public bool IsStop { get; set; }
    }
}
