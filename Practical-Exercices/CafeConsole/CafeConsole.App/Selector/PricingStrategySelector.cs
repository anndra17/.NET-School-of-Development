using CafeConsole.App.Abstractions;
using CafeConsole.Domain.Abstractions;
using CafeConsole.Domain.Models.Pricing;

namespace CafeConsole.App.Selector;

public sealed class PricingStrategySelector : IPricingStrategySelector
{
    private readonly IEnumerable<IPricingStrategy> _strategies;
    public PricingStrategySelector(IEnumerable<IPricingStrategy> strategies)
        => _strategies = strategies;

    public IPricingStrategy Resolve(PricingPolicy policy)
       => _strategies.FirstOrDefault(s => s.Policy == policy)
       ?? throw new InvalidOperationException($"No pricing strategy registered for policy '{policy}'. " +
           $"Registered strategies: {string.Join(", ", _strategies.Select(s => s.Policy))}");
}