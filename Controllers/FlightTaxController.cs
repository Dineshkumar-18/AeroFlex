using AeroFlex.Dtos;
using AeroFlex.Models;
using AeroFlex.Repository.Contracts;
using AeroFlex.Response;
using Microsoft.AspNetCore.Mvc;

namespace AeroFlex.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FlightTaxController : ControllerBase
    {
        private readonly IFlightTaxRepository _flightTaxRepository;
        private readonly IClassRepository _classRepository;
        private readonly ICountryRepository _countryRepository;

        public FlightTaxController(
            IFlightTaxRepository flightTaxRepository,
            IClassRepository classRepository,
            ICountryRepository countryRepository)
        {
            _flightTaxRepository = flightTaxRepository;
            _classRepository = classRepository;
            _countryRepository = countryRepository;
        }

        [HttpPost("addTax")]
        public async Task<ActionResult<GeneralResponse>> AddTax([FromBody] FlightTaxDto flightTaxDto)
        {
            if (!ModelState.IsValid) return BadRequest("Model is invalid");

            var country = await _countryRepository.GetCountryByNameAsync(flightTaxDto.CountryName);
            if (country == null) return BadRequest("Country not found");

            var countryId = country.CountryId;
            var travelType = flightTaxDto.TravelType;
            var classes = flightTaxDto.ClassName == "All"
                ? await _classRepository.GetAllClassesAsync()
                : new List<Class> { await _classRepository.GetClassByNameAsync(flightTaxDto.ClassName) };

            if (classes.Count == 0) return BadRequest("Class not found");

            // Check for existing tax records
            var existingTaxes = await _flightTaxRepository.GetExistingTaxesAsync(
                countryId,
                classes.Select(c => c.ClassId).ToList(),
                travelType
            );
            if (existingTaxes == null) return BadRequest("Error occured check the model");

            var newTaxes = new List<FlightTax>();
            foreach (var flightClass in classes)
            {
                if (!existingTaxes.Any(et => et.ClassId == flightClass.ClassId))
                {
                    newTaxes.Add(new FlightTax
                    {
                        CountryId = countryId,
                        ClassId = flightClass.ClassId,
                        TravelType =(TravelType)Enum.Parse(typeof(TravelType), travelType, true),
                        TaxRate = flightTaxDto.TaxRate
                    });
                }
            }

            if (!newTaxes.Any())
            {
                return BadRequest("Taxes already exist for the specified criteria.");
            }

            var result = await _flightTaxRepository.AddBulkFlightTaxesAsync(newTaxes);

            if (!result)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while adding taxes.");
            }

            return Ok(new GeneralResponse(true, "Taxes added successfully"));
        }
    }

}
