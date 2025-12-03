using AutoMapper;
using HotelListing.API.Contracts;
using HotelListing.API.Data;
using HotelListing.API.Exceptions;
using HotelListing.API.Models.Country;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CountriesController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly ILogger<CountriesController> _logger;
    private readonly ICountriesRepository _countriesRepository;

    public CountriesController(ICountriesRepository countriesRepository, IMapper mapper, ILogger<CountriesController> logger)
    {
        _countriesRepository = countriesRepository;
        _mapper = mapper;
        _logger = logger;
    }

    // GET: api/Countries
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetCountryDto>>> GetCountries()
    {
            var countries = await _countriesRepository.GetAllAsync();

            var records = _mapper.Map<List<GetCountryDto>>(countries);

            return Ok(records);
    }

    // GET: api/Countries/5
    [HttpGet("{id}")]
    public async Task<ActionResult<CountryDto>> GetCountry(int id)
    {
        
            var country = await _countriesRepository.GetDetails(id);

            if (country == null)
            {
                throw new NotFoundException(nameof(GetCountry), id);
            }

            var countryDto = _mapper.Map<CountryDto>(country);

            return Ok(countryDto);
        
    }

    // PUT: api/Countries/5
    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> PutCountry(int id, UpdateCountryDto updateCountryDto)
    {
        _logger.LogInformation("PutCountry request received for country with ID: {Id}.", id);

        if (id != updateCountryDto.Id)
        {
            _logger.LogWarning(
                            "PutCountry validation failed. Route ID {RouteId} does not match body ID {BodyId}.",
                            id, updateCountryDto.Id);
            return BadRequest("The ID in the URL does not match the ID in the request body.");
        }

        var country = await _countriesRepository.GetAsync(id);

        if (country == null)
        {
            throw new NotFoundException(nameof(GetCountry), id);
        }

        _mapper.Map(updateCountryDto, country);

        try
        {
            await _countriesRepository.UpdateAsync(country);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            if (!await CountryExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // POST: api/Countries
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Country>> PostCountry(CreateCountryDto createCountry)
    {
            var country = _mapper.Map<Country>(createCountry);

            await _countriesRepository.AddAsync(country);

            _logger.LogInformation(
                "PostCountry successful. Country created with ID: {Id} and name: {Name}.",
                country.Id, country.Name);

            return CreatedAtAction(nameof(GetCountry), new { id = country.Id }, country);
        
    }

    // DELETE: api/Countries/5
    [HttpDelete("{id}")]
    [Authorize(Roles ="Administrator")]
    public async Task<IActionResult> DeleteCountry(int id)
    {
        
            var country = await _countriesRepository.GetAsync(id);

            if (country == null)
            {
                throw new NotFoundException(nameof(GetCountry), id);
            }

            await _countriesRepository.DeleteAsync(id);

            return NoContent();
    }

    private async Task<bool> CountryExists(int id)
    {
        return await _countriesRepository.Exists(id);
    }
}
