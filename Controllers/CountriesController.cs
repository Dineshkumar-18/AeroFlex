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
using AeroFlex.Repository.Contracts;
using Microsoft.AspNetCore.Authorization;

namespace AeroFlex.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        private readonly ICountryRepository _countryRepository;

        public CountryController(ICountryRepository countryRepository)
        {
            _countryRepository = countryRepository;
        }

        // GET: api/Country
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CountryDto>>> GetCountries()
        {
            var countries = await _countryRepository.GetAllCountriesAsync();
            return Ok(countries);
        }

        // GET: api/Country/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<CountryDto>> GetCountry(int id)
        {
            var country = await _countryRepository.GetCountryByIdAsync(id);

            if (country == null)
            {
                return NotFound();
            }

            return Ok(country);
        }

        // POST: api/Country
        [HttpPost]
        [Authorize(Roles="Admin")]
        public async Task<ActionResult<CountryDto>> PostCountry(CountryDto countryDto)
        {
            var createdCountry = await _countryRepository.CreateCountryAsync(countryDto);
            return CreatedAtAction(nameof(GetCountry), new { id = createdCountry.CountryName }, createdCountry);
        }

        // PUT: api/Country/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutCountry(int id, CountryDto countryDto)
        {
            var updatedCountry = await _countryRepository.UpdateCountryAsync(id, countryDto);
            if (updatedCountry == null)
            {
                return NotFound();
            }

            return NoContent();
        }

        // DELETE: api/Country/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCountry(int id)
        {
            var deleted = await _countryRepository.DeleteCountryAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }

}
