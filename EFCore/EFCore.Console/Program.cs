using EFCore.Data.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System.Transactions;

// 1. Instance of ctx
using var context = new FootballLeagueDbContext();

// Select all teams
// await GetAllTeams();
// await GetAllTeamsQuerySyntax();

// Select one team
//await GetOneTeam();

// Select all records from the table that meets a condition
await GetFilteredTeams();

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
   
async Task GetFilteredTeams()
{
    Console.WriteLine("Enter Search Term: ");
    var searchTerm = Console.ReadLine();

    var teamsFiltered = await context.Teams.Where(t => t.Name == searchTerm)
      .ToListAsync();

    foreach (var team in teamsFiltered)
    {
        Console.WriteLine(team.Name);
    }

    // var partialMatches = await context.Teams.Where(t => t.Name.Contains(searchTerm)).ToListAsync();

    // select * from Teams WHERE Name LIKE '%FC%'
    var partialMatches = await context.Teams.Where(t => EF.Functions.Like(t.Name, $"%{searchTerm}%")).ToListAsync();

    foreach (var team in partialMatches)
    {
        Console.WriteLine(team.Name);
    }
}

async Task GetAllTeamsQuerySyntax()
{
    Console.WriteLine("Enter Desired Team: ");
    var desiredTeam = Console.ReadLine();

    var teams = await (from team in context.Teams
                       where EF.Functions.Like(team.Name, $"%{desiredTeam}%")
                       select team)
                     .ToListAsync();

    foreach (var team in teams)
    {
        Console.WriteLine(team.Name);
    }
}