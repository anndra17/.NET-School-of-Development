using CafeConsole.Domain.Abstractions;
using CafeConsole.Domain.Models.Pricing;

namespace CafeConsole.App.Abstractions;

public interface IPricingStrategySelector
{
    IPricingStrategy Resolve(PricingPolicy policy);
}