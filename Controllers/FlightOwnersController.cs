using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AeroFlex.Data;
using AeroFlex.Models;
using AeroFlex.Dtos;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using AeroFlex.Repository.Contracts;

namespace AeroFlex.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles="FlightOwner")]
    [ApiController]
    public class FlightOwnersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly IFlightOwnerAccount _flightOwnerAccount;
        public FlightOwnersController(ApplicationDbContext context,IWebHostEnvironment environment,IFlightOwnerAccount flightOwnerAccount)
        {
            _context = context;
            _environment = environment;
            _flightOwnerAccount = flightOwnerAccount;
        }

        // GET: api/FlightOwners
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FlightOwner>>> GetFlightOwners()
        {
            return await _context.FlightOwners.ToListAsync();
        }

        // GET: api/FlightOwners/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FlightOwner>> GetFlightOwner(int id)
        {
            var flightOwner = await _context.FlightOwners.FindAsync(id);

            if (flightOwner == null)
            {
                return NotFound();
            }

            return flightOwner;
        }

        // PUT: api/FlightOwners/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFlightOwner(int id, FlightOwnerProfileUpdateDto flightOwner)
        {

            var flightowner = await _context.FlightOwners.FirstOrDefaultAsync(fo => fo.UserId == id);
            if (flightowner == null) return BadRequest("FlightOwner not found");
            var checkByEmail=await _flightOwnerAccount.FindByEmail(flightOwner.Email);
            if(checkByEmail!=null && checkByEmail.Email != flightOwner.Email)
            {
                return BadRequest("Email already exist");
            }
            var checkByUsername = await _flightOwnerAccount.FindByUserName(flightOwner.UserName);
            if (checkByUsername != null && checkByUsername.UserName != flightOwner.UserName)
            {
                return BadRequest("UserName already exists");
            }

            flightowner.PhoneNumber = flightOwner.PhoneNumber;
            flightowner.DateOfBirth = flightOwner.DateOfBirth;
            flightowner.CompanyPhoneNumber = flightOwner.CompanyPhoneNumber;
            flightowner.CompanyEmail = flightOwner.CompanyEmail;
            flightowner.SupportContact = flightOwner.SupportContact;
            flightowner.UserName = flightOwner.UserName;
            flightowner.Email = flightOwner.Email;
            flightowner.FirstName = flightOwner.FirstName;
            flightowner.LastName = flightOwner.Lastname;
            flightowner.PhoneNumber=flightOwner.PhoneNumber;
            flightowner.DateOfBirth=flightOwner.DateOfBirth;
            flightowner.ProfilePictureUrl = flightOwner.ProfilePictureUrl;
            flightowner.AddressId = flightOwner.AddressId;

            _context.Update(flightowner);
            await _context.SaveChangesAsync();

            return Ok("Your profile updated successfully");
        }

        // POST: api/FlightOwners
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<FlightOwner>> PostFlightOwner(FlightOwner flightOwner)
        {
            _context.FlightOwners.Add(flightOwner);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFlightOwner", new { id = flightOwner.UserId }, flightOwner);
        }

        // DELETE: api/FlightOwners/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFlightOwner(int id)
        {
            var flightOwner = await _context.FlightOwners.FindAsync(id);
            if (flightOwner == null)
            {
                return NotFound();
            }

            _context.FlightOwners.Remove(flightOwner);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FlightOwnerExists(int id)
        {
            return _context.FlightOwners.Any(e => e.UserId == id);
        }

        [HttpPut]
        [Route("profileCompletion/{flightOwnerId}")]
        public async Task<ActionResult> ProfileCompletion(int flightOwnerId,FlightOwnerProfile profile)
        {
            var flightowner = await _context.FlightOwners.FirstOrDefaultAsync(fo => fo.UserId == flightOwnerId);
            if (flightowner == null) return BadRequest("FlightOwner Not found");

            if (flightowner.IsApproved)
            {
                return BadRequest("Your profile has already approved by admin you can schedule your flights");
            }

            if(!flightowner.IsApproved && !flightowner.IsProfileCompleted)
            {
                flightowner.CompanyName = profile.CompanyName;
                flightowner.CompanyRegistrationNumber= profile.CompanyRegistrationNumber;
                flightowner.CompanyEmail = profile.CompanyEmail;
                flightowner.CompanyPhoneNumber = profile.CompanyPhoneNumber;
                flightowner.OperatingLicenseNumber= profile.OperatingLicenseNumber;
                flightowner.IsProfileCompleted = true;
                _context.FlightOwners.Update(flightowner);
                await _context.SaveChangesAsync();
                return Ok("Profile updated successfully! it is under review of admin");
            }
            return BadRequest("Your profile is waiting for admin approval");

        }


     
        [HttpGet("get-flightowner-details")]
          // Ensure the user is authenticated
        public IActionResult GetFlightOwnerDetails()
        {

            var flightOwnerid = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;

            if (flightOwnerid==null)
            {
                return Unauthorized("Flightowner ID not found in token.");
            }

            var flightOwnerId = int.Parse(flightOwnerid);

            

            return Ok(new { flightOwnerId });
        }



        [HttpGet]
        [Route("getallflightinfo/{id}")]
        public async Task<ActionResult> FlightInfo(int id)
        {
            var flights = await (from airline in _context.Airlines
                           join flight in _context.Flights
                           on airline.AirlineId equals flight.AirlineId
                           where airline.FlightOwnerId == id
                           select new
                           {
                              flight.FlightId,
                              flight.FlightNumber,
                              airline.AirlineName,
                              FlightType = flight.FlightType.ToString(),
                              flight.TotalSeats,
                              DepartureAirport=flight.DepartureAirport.AirportName,
                              ArrivalAirport=flight.ArrivalAirport.AirportName
                           }).ToListAsync();



            if (flights.Count == 0)
            {
                return NotFound();
            }
            return Ok(flights);    
        }

        // POST: api/upload-logo
        [HttpPost("upload-profile-picture/{userId}")]
        public async Task<IActionResult> UploadProfilePicture(int userId,IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is not selected");

            try
            {
                string uploadPath = Path.Combine(_environment.WebRootPath, "profile-pictures");

                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                // Check if the user already exists (but without a profile picture)
                var flightOwner = await _context.FlightOwners.FirstOrDefaultAsync(fo => fo.UserId == userId);
                if (flightOwner == null)
                    return NotFound("Flight owner not found");

                if (!string.IsNullOrEmpty(flightOwner.ProfilePictureUrl))
                    return BadRequest("Profile picture already exists. Use PUT to update.");

                // Handle file upload and save the URL
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                string fileUrl = $"{Request.Scheme}://{Request.Host}/profile-pictures/{fileName}";

                flightOwner.ProfilePictureUrl = fileUrl;
                await _context.SaveChangesAsync();

                return Ok(new { fileUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpPut("update-profile-picture/{userId}")]
        public async Task<IActionResult> UpdateProfilePicture(int userId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is not selected");

            try
            {
                string uploadPath = Path.Combine(_environment.WebRootPath, "profile-pictures");

                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                var flightOwner = await _context.FlightOwners.FirstOrDefaultAsync(fo => fo.UserId == userId);
                if (flightOwner == null)
                    return NotFound("Flight owner not found");

                if (string.IsNullOrEmpty(flightOwner.ProfilePictureUrl))
                    return BadRequest("No profile picture exists. Use POST to upload.");

                // Delete existing profile picture
                string existingFileName = Path.GetFileName(flightOwner.ProfilePictureUrl);
                string existingFilePath = Path.Combine(uploadPath, existingFileName);

                if (System.IO.File.Exists(existingFilePath))
                    System.IO.File.Delete(existingFilePath);

                // Upload new profile picture
                string newFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string newFilePath = Path.Combine(uploadPath, newFileName);

                using (var stream = new FileStream(newFilePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                string newFileUrl = $"{Request.Scheme}://{Request.Host}/profile-pictures/{newFileName}";

                flightOwner.ProfilePictureUrl = newFileUrl;
                await _context.SaveChangesAsync();

                return Ok(new { newFileUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }


        [HttpPut("update-address/{flightOwnerId}")]
        public async Task<IActionResult> UpdateFlightOwnerAddress(int flightOwnerId, [FromBody] int addressId)
        {
            try
            {
                // Retrieve the FlightOwner from the database
                var flightOwner = await _context.FlightOwners.FirstOrDefaultAsync(fo => fo.UserId == flightOwnerId);
                if (flightOwner == null)
                    return NotFound("Flight owner not found.");

                // Check if the provided AddressId exists in the Address table
                var address = await _context.Addresses.FirstOrDefaultAsync(a => a.AddressId == addressId);
                if (address == null)
                    return NotFound("Address not found.");

                // Update the FlightOwner's AddressId field
                flightOwner.AddressId = addressId;
                await _context.SaveChangesAsync();

                return Ok("FlightOwner's address updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }


    }
}
