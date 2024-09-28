using AeroFlex.Dtos;

namespace AeroFlex.Response
{
    public class FlightSearchResponse
    {
        public List<FlightScheduleDTO> OutboundFlights { get; set; }
        public List<FlightScheduleDTO> ReturnFlights { get; set; }
    }
}
