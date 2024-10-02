using AeroFlex.Dtos;
using AeroFlex.Repository.Contracts;
using AeroFlex.Repository.Implementations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AeroFlex.Controllers
{   
    [Route("api/[controller]")]
    [ApiController]
    public class CancellationController(ICancellationRepository cancellationRepository) : ControllerBase
    {
            [HttpPost]
            [Route("CancelFlight/{flightScheduleId}")]
            public async Task<ActionResult> CancellationProcess(int flightScheduleId,Cancel cancellation)
            {

            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Return validation errors if any
            }


            var userId=HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not authenticated.");
            }

            var SuccessCancelling=await cancellationRepository.CancellationProcess(cancellation, int.Parse(userId),flightScheduleId);
                if(!SuccessCancelling.flag) return BadRequest(SuccessCancelling.message);
                return Ok(SuccessCancelling);
            }
    }
}
