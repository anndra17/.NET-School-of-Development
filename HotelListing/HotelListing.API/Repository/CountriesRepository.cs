using HotelListing.API.Contracts;
using HotelListing.API.Data;

namespace HotelListing.API.Repository;

public class CountriesRepository : GenericRepository<Country>, ICountriesRepository
{
    private readonly HotelListingDbContext _context;
    public CountriesRepository(HotelListingDbContext context) : base(context)
    {
        _context = context;
    }
}
