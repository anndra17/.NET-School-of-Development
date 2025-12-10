using EFCore.Data.Repository;
using Microsoft.EntityFrameworkCore;

// 1. Instance of ctx
using var context = new FootballLeagueDbContext();

// Select all teams
// await GetAllTeams();

// Select one team
await GetOneTeam();

async Task GetAllTeams()
{
    // Select all teams
    // SEECT * FROM Teams
    var teams = await context.Teams.ToListAsync();

    foreach (var team in teams)
    {
        Console.WriteLine(team.Name);
    }
}

async Task GetOneTeam()
{
    // Selecting a single record - first one in the list
    var teamFirst = await context.Teams.FirstAsync();
    if (teamFirst != null)
    {
        Console.WriteLine(teamFirst.Name);
    }

    var teamFirstOrDefault = await context.Teams.FirstOrDefaultAsync();
    if (teamFirstOrDefault != null)
    {
        Console.WriteLine(teamFirstOrDefault.Name);
    }

    // Selecting a single record - First one in the list that meets a condition
    var teamFirstWithCondition = await context.Teams.FirstAsync(t => t.Id == 1);
    if (teamFirstWithCondition != null)
    {
        Console.WriteLine(teamFirstWithCondition.Name);
    }
    var teamFirstOrDefaultWithCondition = await context.Teams.FirstOrDefaultAsync(t => t.Id == 2);
    if (teamFirstOrDefaultWithCondition != null)
    {
        Console.WriteLine(teamFirstOrDefaultWithCondition.Name);
    }

    // Selecting a single record = Only one record should be returned, or an exception will be thrown
    var teamSingle = await context.Teams.SingleAsync();
    if (teamSingle != null)
    {
        Console.WriteLine(teamSingle.Name);
    }
    var teamSingleWithCondition = await context.Teams.SingleAsync(t => t.Id == 3);
    if (teamSingleWithCondition != null)
    {
        Console.WriteLine(teamSingleWithCondition.Name);
    }
    var teamSingleOrDefaultWithCondition = await context.Teams.SingleOrDefaultAsync(t => t.Id == 1);
    if (teamSingleOrDefaultWithCondition != null)
    {
        Console.WriteLine(teamSingleOrDefaultWithCondition.Name);
    }
    var teamSingleOrDefault = await context.Teams.SingleOrDefaultAsync();
    if (teamSingleOrDefault != null)
    {
        Console.WriteLine(teamSingleOrDefault.Name);
    }

    // Selecting based on Primary Key Id value
    var teamBasedOnId = await context.Teams.FindAsync(1);
    if (teamBasedOnId != null)
    {
        Console.WriteLine(teamBasedOnId.Name);
    }
}
   