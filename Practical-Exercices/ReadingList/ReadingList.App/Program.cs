using Microsoft.Extensions.DependencyInjection;
using ReadingList.CLI.App;
using ReadingList.Infrastructure;

var services = new ServiceCollection()
    .AddUi()
    .AddInfrastructure()
    .AddUiPolicies()
    .AddCommands()
    .AddCliRunner();

using var provider = services.BuildServiceProvider(new ServiceProviderOptions
{
    ValidateOnBuild = true,
    ValidateScopes  = true
});

var app = provider.GetRequiredService<CliApplication>();

using var cancellationToken = new CancellationTokenSource();
Console.CancelKeyPress += (s, e) =>
{
    e.Cancel = true;
    app.CancelCurrentOperation();
    Console.WriteLine("Operation canceled...");
};

await app.RunAsync(cancellationToken.Token);
