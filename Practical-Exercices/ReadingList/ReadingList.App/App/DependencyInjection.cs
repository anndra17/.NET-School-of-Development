using Microsoft.Extensions.DependencyInjection;
using ReadingList.Application.Abstractions.IO;
using ReadingList.Application.Abstractions.Logs;
using ReadingList.CLI.Adapters;
using ReadingList.CLI.Commands.Parsing;
using ReadingList.CLI.Commands.Routing;
using ReadingList.CLI.Commands.Routing.Abstractions;
using ReadingList.CLI.UI.Abstractions;
using ReadingList.CLI.UI.Implementations;
using ReadingList.CLI.UI.Rendering;

namespace ReadingList.CLI.App;

public static class DependencyInjection
{
    public static IServiceCollection AddUiPolicies(this IServiceCollection services)
    {
        services.AddSingleton<IConflictResolver, ConsoleConflictResolver>();
        services.AddSingleton<IOverwritePolicy, ConsoleOverwritePolicy>();
        services.AddSingleton<ISystemLogger>(sp =>
           new InfrastructureLoggerAdapter(sp.GetRequiredService<IConsoleNotifier>()));

        return services;
    }

    public static IServiceCollection AddCommands(this IServiceCollection services)
    {
        services.AddSingleton<CommandParser>();
        services.AddSingleton<CommandRouter>();

        services.AddSingleton<IImportHandler, ImportHandler>();
        services.AddSingleton<IQueryHandler, QueryHandler>();
        services.AddSingleton<IUpdateHandler, UpdateHandler>();
        services.AddSingleton<IExportHandler, ExportHandler>();
        services.AddSingleton<IHelpHandler, HelpHandler>();
        services.AddSingleton<IExitHandler, ExitHandler>();

        return services;
    }

    public static IServiceCollection AddUi(this IServiceCollection services)
    {
        services.AddSingleton<IConsole, SystemConsoleAdapter>();
        services.AddSingleton<IConsoleNotifier, ConsoleNotifier>();
        services.AddSingleton<BookTableRenderer>();
        services.AddSingleton<BookStatsRenderer>();

        return services;
    }

    public static IServiceCollection AddCliRunner(this IServiceCollection services)
    {
        services.AddSingleton<CliApplication>();

        return services;
    }
}
