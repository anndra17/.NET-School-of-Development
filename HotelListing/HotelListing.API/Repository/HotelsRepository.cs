using AutoMapper;
using HotelListing.API.Contracts;
using HotelListing.API.Data;

namespace HotelListing.API.Repository
{
    public class HotelsRepository : GenericRepository<Hotel>, IHotelRepository
    {
        public HotelsRepository(HotelListingDbContext context, IMapper mapper) : base(context, mapper)
        {
        }
    }
}
