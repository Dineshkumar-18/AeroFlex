using AeroFlex.Data;
using AeroFlex.Dtos;
using AeroFlex.Models;
using AeroFlex.Repository.Contracts;
using Microsoft.EntityFrameworkCore;

namespace AeroFlex.Repository.Implementations
{
    public class ItineraryService : IItineraryService
    {
        private readonly ApplicationDbContext _context;
        private readonly IFlightSegmentService _flightSegmentService;

        public ItineraryService(ApplicationDbContext context, IFlightSegmentService flightSegmentService)
        {
            _context = context;
            _flightSegmentService = flightSegmentService;
        }

        public Itinerary CreateItinerary(ItineraryDTO itineraryDto)
        {

            var startAirport=_context.Airports.FirstOrDefault(a=>a.AirportName== itineraryDto.StartAirportName);
            var endAirport = _context.Airports.FirstOrDefault(a => a.AirportName == itineraryDto.EndAiportName);

            var itinerary = new Itinerary
            {
                StartAirportId = startAirport.AirportId,
                EndAirportId = endAirport.AirportId,
                TotalStops = itineraryDto.TotalStops
            };

            _context.Itineraries.Add(itinerary);
            _context.SaveChanges();
            return itinerary;
        }

        public Itinerary GetItinerary(int id)
        {
             var itinery= _context.Itineraries
                .Include(i => i.FlightSegments)
                .ThenInclude(fs => fs.FlightSchedule)
                .FirstOrDefault(i => i.ItineraryId == id);

            return itinery!;
        }

        public void FinalizeItinerary(int id)
        {
            var itinerary = _context.Itineraries.Find(id);
            if (itinerary == null) throw new Exception("Itinerary not found");



           
            // Optionally not // Add logic to mark itinerary as finalizedify admins or owners

            _context.SaveChanges();
        }
    }
}
