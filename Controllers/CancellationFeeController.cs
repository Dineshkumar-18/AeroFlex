using AeroFlex.Dtos;
using AeroFlex.Models;
using AeroFlex.Repository.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AeroFlex.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CancellationFeeController : ControllerBase
    {
        private readonly ICancellationFeeRepository _repository;

        public CancellationFeeController(ICancellationFeeRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> GetAll()
        {
            var cancellationFees = await _repository.GetAllAsync();
            return Ok(cancellationFees);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var cancellationFee = await _repository.GetByIdAsync(id);

            if (cancellationFee == null)
            {
                return NotFound();
            }

            return Ok(cancellationFee);
        }

        [HttpPost]
        [Authorize(Roles ="FlightOwner")]
        public async Task<IActionResult> Create(CancellationFeeDto dto)
        {
            try
            {
                var createdCancellationFee = await _repository.CreateAsync(dto);
                return Ok(createdCancellationFee);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles="FlightOwner")]
        public async Task<IActionResult> Update(int id, CancellationFeeDto dto)
        {
            try
            {
                var updatedCancellationFee = await _repository.UpdateAsync(id, dto);
                if (updatedCancellationFee == null)
                {
                    return NotFound();
                }

                return Ok(updatedCancellationFee);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "FlightOwner")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _repository.DeleteAsync(id);

            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpGet]
        [Route("getplatformfee")]
        public async Task<ActionResult> GetPlatformFee([FromQuery] int flightScheduleId)
        {
            var platformFee = await _repository.GetPlatformFee(flightScheduleId);
            return Ok(platformFee);
        }

    }

}
