using AutoMapper;
using HotelListing.API.Contracts;
using HotelListing.API.Data;
using HotelListing.API.Models.Country;
using HotelListing.API.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

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
        _logger.LogInformation("GetCountries request received.");

        try
        {
            var countries = await _countriesRepository.GetAllAsync();

            var records = _mapper.Map<List<GetCountryDto>>(countries);

            return Ok(records);
        }
        catch (Exception ex) 
        {
            _logger.LogError(ex, $"Something Went Wrong in the {nameof(GetCountries)}");
            return Problem($"Something Went Wrong in the {nameof(GetCountries)}", statusCode: 500);
        }
    }

    // GET: api/Countries/5
    [HttpGet("{id}")]
    public async Task<ActionResult<CountryDto>> GetCountry(int id)
    {
        _logger.LogInformation("GetCountry request received for country with ID: {Id}.", id);

        try
        {
            var country = await _countriesRepository.GetDetails(id);

            if (country == null)
            {
                _logger.LogWarning("No record found in {ActionName} for ID: {Id}.", nameof(GetCountry), id);
                return NotFound();
            }

            var countryDto = _mapper.Map<CountryDto>(country);

            return Ok(countryDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Something Went Wrong in the {nameof(GetCountry)} - GetCountry Attempt with ID: {id}");
            return Problem($"Something Went Wrong in the {nameof(GetCountry)}", statusCode: 500);
        }
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
            _logger.LogWarning("PutCountry failed. Country with ID: {Id} was not found.", id);
            return NotFound();
        }

        _mapper.Map(updateCountryDto, country);

        try
        {
            await _countriesRepository.UpdateAsync(country);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogError(ex, "Concurrency error in {ActionName} for ID: {Id}.", nameof(PutCountry), id);

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
        _logger.LogInformation("PostCountry request received.");

        try
        {
            var country = _mapper.Map<Country>(createCountry);

            await _countriesRepository.AddAsync(country);

            _logger.LogInformation(
                "PostCountry successful. Country created with ID: {Id} and name: {Name}.",
                country.Id, country.Name);

            return CreatedAtAction(nameof(GetCountry), new { id = country.Id }, country);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Something went wrong in {ActionName}.", nameof(PostCountry));
            return Problem($"Something went wrong in {nameof(PostCountry)}", statusCode: 500);
        }
    }

    // DELETE: api/Countries/5
    [HttpDelete("{id}")]
    [Authorize(Roles ="Administrator")]
    public async Task<IActionResult> DeleteCountry(int id)
    {
        _logger.LogInformation("DeleteCountry request received for country with ID: {Id}.", id);

        try
        {
            var country = await _countriesRepository.GetAsync(id);
            if (country == null)
            {
                _logger.LogWarning("DeleteCountry failed. Country with ID: {Id} was not found.", id);
                return NotFound();
            }

            await _countriesRepository.DeleteAsync(id);

            _logger.LogInformation("DeleteCountry successful for country with ID: {Id}.", id);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Something went wrong in {ActionName} for ID: {Id}.", nameof(DeleteCountry), id);
            return Problem($"Something went wrong in {nameof(DeleteCountry)}", statusCode: 500);
        }
    }

    private async Task<bool> CountryExists(int id)
    {
        return await _countriesRepository.Exists(id);
    }
}
