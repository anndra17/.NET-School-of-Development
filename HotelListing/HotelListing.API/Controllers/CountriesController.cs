using AutoMapper;
using HotelListing.API.Core.Contracts;
using HotelListing.API.Core.Models;
using HotelListing.API.Core.Models.Country;
using HotelListing.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CountriesController : ControllerBase
{
    private readonly ICountriesRepository _countriesRepository;
    private readonly ILogger<CountriesController> _logger;
    private readonly IMapper _mapper;
    public CountriesController(ICountriesRepository countriesRepository, IMapper mapper, ILogger<CountriesController> logger)
    {
        _countriesRepository = countriesRepository;
        _mapper = mapper;
        _logger = logger;
    }

    // DELETE: api/Countries/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> DeleteCountry(int id)
    {
        await _countriesRepository.DeleteAsync(id);

        return NoContent();
    }

    // GET: api/Countries/GetAll
    [HttpGet("GetAll")]
    [EnableQuery]
    public async Task<ActionResult<IEnumerable<GetCountryDto>>> GetCountries()
    {
        var countries = await _countriesRepository.GetAllAsync<GetCountryDto>();

        return Ok(countries);
    }

    // GET: api/Countries/5
    [HttpGet("{id}")]
    public async Task<ActionResult<CountryDto>> GetCountry(int id)
    {
        var country = await _countriesRepository.GetDetails(id);

        return Ok(country);
    }

    // GET: api/Countries/?StartIndex=0&pagesize=25&PageNumber=1
    [HttpGet]
    public async Task<ActionResult<PagedResult<GetCountryDto>>> GetPagedCountries([FromQuery] QueryParameters queryParameters)
    {
        var pagedCountriesResult = await _countriesRepository.GetAllAsync<GetCountryDto>(queryParameters);

        return Ok(pagedCountriesResult);
    }
    // POST: api/Countries
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Country>> PostCountry(CreateCountryDto createCountryDto)
    {
        var country = await _countriesRepository.AddAsync<CreateCountryDto, GetCountryDto>(createCountryDto);

        return CreatedAtAction(nameof(GetCountry), new { id = country.Id }, country);
    }

    // PUT: api/Countries/5
    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> PutCountry(int id, UpdateCountryDto updateCountryDto)
    {
        if (id != updateCountryDto.Id)
        {
            return BadRequest("The ID in the URL does not match the ID in the request body.");
        }

        try
        {
            await _countriesRepository.UpdateAsync(id, updateCountryDto);
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
    private async Task<bool> CountryExists(int id)
    {
        return await _countriesRepository.Exists(id);
    }
}
