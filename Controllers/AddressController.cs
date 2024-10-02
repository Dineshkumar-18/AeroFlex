using AeroFlex.Data;
using AeroFlex.Dtos;
using AeroFlex.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AeroFlex.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AddressController(ApplicationDbContext _context) : ControllerBase
    {
        [HttpPost("create-address")]
        public async Task<IActionResult> CreateAddress([FromBody] AddressDto addressDto)
        {
            if (addressDto == null)
                return BadRequest("Address details are required.");

            try
            {
                // Create a new Address entity from the provided DTO
                var newAddress = new Address
                {
                    StreetAddress = addressDto.StreetAddress,
                    City = addressDto.City,
                    State = addressDto.State,
                    Zipcode = addressDto.Zipcode,
                    Country = addressDto.Country
                };

                // Save the new Address to the database
                _context.Addresses.Add(newAddress);
                await _context.SaveChangesAsync();

                // Return the newly created AddressId
                return Ok(new { AddressId = newAddress.AddressId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetAddressById(int id)
        {
            var address = await _context.Addresses.FirstOrDefaultAsync(a => a.AddressId == id);
            if(address==null)
            {
                return BadRequest("Address Not found");
            }
            var addressInfo = new AddressDto
            {
               StreetAddress=address.StreetAddress,
               City=address.City,
               Country=address.Country,
               State=address.State,
               Zipcode=address.Zipcode
            };
            return Ok(addressInfo);
        }

        [HttpPut]
        [Route("update-address/{id}")]
        public async Task<IActionResult> UpdateAddressById(int id,AddressDto addressDto)
        {
            var userId = getIdFromToken();
            if (userId == -1) return Unauthorized();
            var userInfo = await  _context.Users.FirstOrDefaultAsync(u => u.AddressId == id);
            if (userInfo == null) return BadRequest("user not found");
            if (userInfo.UserId != userId) return Forbid();
            var address = await _context.Addresses.FirstOrDefaultAsync(a => a.AddressId == id);
            if (address == null) return BadRequest("Address not found");

            address.StreetAddress = addressDto.StreetAddress;
            address.City = addressDto.City;
            address.Country = addressDto.Country;
            address.State = addressDto.State;
            address.Zipcode = addressDto.Zipcode;

            _context.Update(address);
            await _context.SaveChangesAsync();

            return Ok("address updated successfully");
        }

        private int getIdFromToken()
        {
            var user = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if(user==null)
            {
                return -1;
            }
            return int.Parse(user);
        }



    }
}
