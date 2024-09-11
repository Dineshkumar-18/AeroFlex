using AeroFlex.Data;
using AeroFlex.Dtos;
using AeroFlex.Models;
using AeroFlex.Repository.Contracts;
using Microsoft.EntityFrameworkCore;

namespace AeroFlex.Repository.Implementations
{
    public class FlightSegmentService : IFlightSegmentService
    {
        private readonly ApplicationDbContext _context;

        public FlightSegmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public FlightSegment AddFlightSegment(FlightSegmentDTO segmentDto)
        {
            var segment = new FlightSegment
            {
                ItineraryId = segmentDto.ItineraryId,
                FlightScheduleId = segmentDto.FlightScheduleId,
                StopOrder = segmentDto.StopOrder,
                IsStop = segmentDto.IsStop
            };

            _context.FlightSegments.Add(segment);
            _context.SaveChanges();
            return segment;
        }

        public IEnumerable<FlightSegment> GetSegmentsByItinerary(int itineraryId)
        {
            return _context.FlightSegments
                .Include(fs => fs.FlightSchedule)
                .Where(fs => fs.ItineraryId == itineraryId)
                .ToList();
        }
    }
}
