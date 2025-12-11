using EFCore.Data.Repository;
using EFCore.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System.Transactions;

// 1. Instance of context
using var context = new FootballLeagueDbContext();

#region Read Queries
// Select all teams
// await GetAllTeams();
// await GetAllTeamsQuerySyntax();

// Select one team
//await GetOneTeam();

// Select all records from the table that meets a condition
//await GetFilteredTeams();

// Aggregate Methods
// await AggregateMethpds();

// Grouping and Aggregating
// await GroupByMethod();

// Ordering 
// await OrderByMethod();

// Skip and Take - Great for Paging
// await SkipAndTake();

// Select and Projections - more precise queries
//await ProjectionsAndSelect();

// No Tracking - EF Core tracks objects that are returned by queries.
// This is less useful in disconnected applications like APIs and web apps
//var teams = await context.Teams
//    .AsNoTracking()
//    .ToListAsync();

#endregion

// INSERT INTO Coaches (cols) VALUES (values)

// Simple insert
var newCoach = new Coach
{
    Name = "Gigi Becali",
    CreatedDate = DateTime.Now,
};

//await context.Coaches.AddAsync(newCoach);
//await context.SaveChangesAsync();

// Loop insert
var newCoach1 = new Coach
{
    Name = "Gigi Becali",
    CreatedDate = DateTime.Now,
};

List<Coach> coaches = new List<Coach>
{
    newCoach,
    newCoach1
};

//foreach (var coach in coaches)
//{
//    await context.Coaches.AddAsync(coach);
//}

//Console.WriteLine(context.ChangeTracker.DebugView.LongView);
//await context.SaveChangesAsync();
//Console.WriteLine(context.ChangeTracker.DebugView.LongView);

//Batch insert
await context.Coaches.AddRangeAsync(coaches);
await context.SaveChangesAsync();

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

async Task AggregateMethpds()
{
    // Count
    var numberOfTeams = await context.Teams.CountAsync();
    Console.WriteLine($"Number of Teams: {numberOfTeams}");

    var numberOfTeamsWithCondition = await context.Teams.CountAsync(t => t.Id == 2);
    Console.WriteLine($"Number of Teams with condition above: {numberOfTeamsWithCondition}");

    // Max
    var maxTeams = await context.Teams.MaxAsync(t => t.Id);

    // Min
    var minTeams = await context.Teams.MinAsync(t => t.Id);

    // Average
    var avgTeams = await context.Teams.AverageAsync(t => t.Id);

    // Sum
    var sumTeams = await context.Teams.SumAsync(t => t.Id);
}

async Task GroupByMethod()
{
    var groupTeams = await context.Teams
        //.Where(t => t.Name == '') // translate to a WHERE clause
        .GroupBy(t => t.CreatedDate.Date)
        //.Where(t => t.Average()) // translate to a HAVING clause
        .ToListAsync();

    foreach (var group in groupTeams)
    {
        Console.WriteLine(group.Key);
        Console.WriteLine(group.Sum(t => t.Id));

        foreach (var team in group)
        {
            Console.WriteLine(team.Name);
        }
    }
}

async Task OrderByMethod()
{
    var orderedTeams = await context.Teams
        .OrderBy(t => t.Name)
        .ToListAsync();

    foreach (var team in orderedTeams)
    {
        Console.WriteLine(team.Name);
    }

    var descOrderedTeams = await context.Teams
        .OrderByDescending(t => t.Name)
        .ToListAsync();

    foreach (var team in descOrderedTeams)
    {
        Console.WriteLine(team.Name);
    }

    // Getting the record with a maximum value
    var maxBy = context.Teams.MaxBy(t => t.Id);
    //or
    var maxByDescendingOrder = await context.Teams
        .OrderByDescending(t => t.Id)
        .FirstOrDefaultAsync();
}

async Task SkipAndTake()
{
    var recordCount = 3;
    var page = 0;
    var next = true;
    while (next)
    {
        var teams = await context.Teams.Skip(page * recordCount).Take(recordCount).ToListAsync();
        foreach (var team in teams)
        {
            Console.WriteLine(team.Name);
        }
        Console.WriteLine("Enter 'true' for the next set of records, 'false' to exit");
        next = Convert.ToBoolean(Console.ReadLine());

        if (!next) break;
        page++;
    }
}

async Task ProjectionsAndSelect()
{
    var teamNames = await context.Teams
    .Select(t => new TeamInfo { Name = t.Name, TeamId = t.Id })
    .ToListAsync();

    foreach (var team in teamNames)
    {
        Console.WriteLine($"{team.Name} - {team.TeamId}");
    }
}
class TeamInfo
{
    public int TeamId { get; set; }
    public string Name { get; set; }
}

