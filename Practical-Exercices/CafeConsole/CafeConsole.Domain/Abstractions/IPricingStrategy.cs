namespace CafeConsole.Domain.Abstractions;

public interface IPricingStrategy
{
    decimal Apply(decimal subtotal);
}
