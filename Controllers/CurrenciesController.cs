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
    [Authorize]
    public class CurrencyController : ControllerBase
    {
        private readonly ICurrencyRepository _currencyRepository;

        public CurrencyController(ICurrencyRepository currencyRepository)
        {
            _currencyRepository = currencyRepository;
        }

        // GET: api/Currency
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CurrencyDto>>> GetCurrencies()
        {
            var currencies = await _currencyRepository.GetAllCurrenciesAsync();
            return Ok(currencies);
        }

        // GET: api/Currency/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<CurrencyDto>> GetCurrency(int id)
        {
            var currency = await _currencyRepository.GetCurrencyByIdAsync(id);

            if (currency == null)
            {
                return NotFound();
            }

            return Ok(currency);
        }

        // GET: api/Currency/country/{countryName}
        [HttpGet("country/{countryName}")]
        public async Task<ActionResult<CurrencyDto>> GetCurrencyByCountry(string countryName)
        {
            var currency = await _currencyRepository.GetCurrencyByCountryAsync(countryName);

            if (currency == null)
            {
                return NotFound();
            }

            return Ok(currency);
        }

        // POST: api/Currency
        [HttpPost]
        public async Task<ActionResult<CurrencyDto>> PostCurrency(CurrencyDto currencyDto)
        {
            var createdCurrency = await _currencyRepository.CreateCurrencyAsync(currencyDto);
            return CreatedAtAction(nameof(GetCurrency), new { id = createdCurrency.CurrencyCode }, createdCurrency);
        }

        // PUT: api/Currency/{id}
        [HttpPut("{id}")]
        [Authorize(Roles="Admin")]
        public async Task<IActionResult> PutCurrency(int id, CurrencyDto currencyDto)
        {
            var updatedCurrency = await _currencyRepository.UpdateCurrencyAsync(id, currencyDto);
            if (updatedCurrency == null)
            {
                return NotFound();
            }

            return NoContent();
        }

        // DELETE: api/Currency/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCurrency(int id)
        {
            var deleted = await _currencyRepository.DeleteCurrencyAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
