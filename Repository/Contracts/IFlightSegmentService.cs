using AeroFlex.Dtos;
using AeroFlex.Models;

namespace AeroFlex.Repository.Contracts
{
    public interface IFlightSegmentService
    {
        FlightSegment AddFlightSegment(FlightSegmentDTO segmentDto);
        IEnumerable<FlightSegment> GetSegmentsByItinerary(int itineraryId);
    }
}
