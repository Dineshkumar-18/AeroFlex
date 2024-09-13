using AeroFlex.Dtos;
using AeroFlex.Repository.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AeroFlex.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CountryTaxController : ControllerBase
    {
        private readonly ICountryTaxService _countryTaxService;

        public CountryTaxController(ICountryTaxService countryTaxService)
        {
            _countryTaxService = countryTaxService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _countryTaxService.GetAllAsync();
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _countryTaxService.GetByIdAsync(id);
            if (!response.flag)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CountryTaxDto countryTaxDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _countryTaxService.AddAsync(countryTaxDto);
            return CreatedAtAction(nameof(GetById), new { id = response.data }, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CountryTaxDto countryTaxDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _countryTaxService.UpdateAsync(id, countryTaxDto);
            if (!response.flag)
            {
                return NotFound(response);
            }

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _countryTaxService.DeleteAsync(id);
            if (!response.flag)
            {
                return NotFound(response);
            }

            return Ok(response);
        }
    }

}
