using AeroFlex.Dtos;
using AeroFlex.Models;

namespace AeroFlex.Repository.Contracts
{
    public interface IItineraryService
    {
        Itinerary CreateItinerary(ItineraryDTO itineraryDto);
        Itinerary GetItinerary(int id);
        void FinalizeItinerary(int id);
    }
}
