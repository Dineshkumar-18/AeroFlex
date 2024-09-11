using AeroFlex.Dtos;
using AeroFlex.Repository.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AeroFlex.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ItineraryController(IItineraryService _itineraryService) : ControllerBase
    {
        [Authorize(Roles ="Admin")]
        [HttpPost("create-itinerary")]
        public IActionResult CreateItinerary(ItineraryDTO dto)
        {
            if(!ModelState.IsValid) return BadRequest("Model is incorrect");
            var itinerary = _itineraryService.CreateItinerary(dto);
            return Ok(itinerary);
        }


    }
}
