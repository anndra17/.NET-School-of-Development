using AirportManagement.Domain.Models;
using AirportManagement.Infrastructure.Persistence.Entities;

namespace AirportManagement.Infrastructure.Mappings;

internal static class FareOfferEntityMappings
{
    public static FareOffer ToDomain(this FareOfferEntity e)
        => new FareOffer
        {
            Id = e.Id,
            FlightScheduleId = e.FlightScheduleId,
            FareClass = FareClassMappings.ToDomain(e.FareClass),
            BasePrice = e.BasePrice,
            Taxes = e.Taxes,
            Currency = e.Currency,
            IsRefundable = e.IsRefundable,
            SeatsAvailable = e.SeatsAvailable
        };

    public static FareOfferEntity ToNewEntity(this FareOffer d)
        => new FareOfferEntity
        {
            FlightScheduleId = d.FlightScheduleId,
            FareClass = FareClassMappings.ToEntity(d.FareClass),
            BasePrice = d.BasePrice,
            Taxes = d.Taxes,
            Currency = d.Currency,
            IsRefundable = d.IsRefundable,
            SeatsAvailable = d.SeatsAvailable
        };
}
