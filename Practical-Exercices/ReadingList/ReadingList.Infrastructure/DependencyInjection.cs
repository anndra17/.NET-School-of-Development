using Microsoft.Extensions.DependencyInjection;
using ReadingList.Application.Abstractions.Csv;
using ReadingList.Application.Abstractions.IO;
using ReadingList.Application.Abstractions.Logs;
using ReadingList.Application.Abstractions.Persistence;
using ReadingList.Application.Services;
using ReadingList.Domain.Entities;
using ReadingList.Infrastructure.Export;
using ReadingList.Infrastructure.FileSystem;
using ReadingList.Infrastructure.Parsing;
using ReadingList.Infrastructure.Persistence;

namespace ReadingList.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IFileSystem, SystemFileSystem>();

        services.AddSingleton<ICsvReader, CsvReader>();
        services.AddSingleton<ICsvBookParser, CsvBookParser>();
        services.AddSingleton<ImportService>();

        services.AddSingleton<CsvExportStrategy>();
        services.AddSingleton<JsonExportStrategy>();
        services.AddSingleton<ExportService>(sp => 
            new ExportService(
                csvStrategy: sp.GetRequiredService<CsvExportStrategy>(),
                jsonStrategy: sp.GetRequiredService<JsonExportStrategy>(),
                fileSystem: sp.GetRequiredService<IFileSystem>(),
                systemLogger: sp.GetRequiredService<ISystemLogger>(),
                overwritePolicy: sp.GetRequiredService<IOverwritePolicy>()
            ));

        services.AddSingleton<BookQueryService>();

        services.AddSingleton<IRepository<Book, int>>(sp => new InMemoryRepository<Book, int>(b => b.Id));
        services.AddSingleton<IUnitOfWork, InMemoryUnitOfWork>();

        return services;
    }
}
