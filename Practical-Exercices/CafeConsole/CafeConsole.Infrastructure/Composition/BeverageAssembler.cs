using CafeConsole.App.Abstractions;
using CafeConsole.App.Dtos;
using CafeConsole.Domain.Abstractions;
using CafeConsole.Domain.Models.Decorators;

namespace CafeConsole.Infrastructure.Composition;

public class BeverageAssembler : IBeverageAssembler
{
    public IBeverage Assemble(IBeverage baseType, IEnumerable<AddOnDto> addOns)
    {
        foreach (var (name, flavor) in addOns)
        {
            baseType = name.ToLower() switch
            {
                "milk" => new MilkDecorator(baseType),
                "extra shot" => new ExtraShotDecorator(baseType),
                "syrup" => new SyrupDecorator(baseType, flavor ?? "vanilla"),
                _ => throw new ArgumentException($"Unknown add-on: {name}")
            };
        }

        return baseType;
    }
}