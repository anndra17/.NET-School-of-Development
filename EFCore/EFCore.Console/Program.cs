using EFCore.Data.Repository;
using EFCore.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System.Transactions;

// 1. Instance of context
using var context = new FootballLeagueDbContext();
await context.Database.MigrateAsync();

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

#endregion

#region Write Queries

// INSERT INTO Coaches (cols) VALUES (values)

// Simple insert
// await InsertOneRecord();

// Loop insert
// await InsertWithLoop();

// Batch insert
// await InsertRange();

// Update operations
// await UpdateWithTracking();
// await UpdateWithNoTracking();

// Delete operations
// await Delete();

// Execute Delete
//await ExecuteDelete();

// Execute Update
// await ExecuteUpdate();


async Task InsertOneRecord()
{
    var newCoach = new Coach
    {
        Name = "Gigi Becali",
        CreatedDate = DateTime.Now,
    };

    await context.Coaches.AddAsync(newCoach);
    await context.SaveChangesAsync();
}

async Task InsertWithLoop()
{
    var newCoach = new Coach
    {
        Name = "Gigi Becali",
        CreatedDate = DateTime.Now,
    };
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

    foreach (var coach in coaches)
    {
        await context.Coaches.AddAsync(coach);
    }

    Console.WriteLine(context.ChangeTracker.DebugView.LongView);
    await context.SaveChangesAsync();
    Console.WriteLine(context.ChangeTracker.DebugView.LongView);
}

async Task InsertRange()
{

    var newCoach = new Coach
    {
        Name = "Gigi Becali",
        CreatedDate = DateTime.Now,
    };
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

    await context.Coaches.AddRangeAsync(coaches);
    await context.SaveChangesAsync();
}

async Task UpdateWithTracking()
{
    var coach = await context.Coaches.FindAsync(9);
    Console.WriteLine(context.ChangeTracker.DebugView.LongView);

    Console.WriteLine(context.ChangeTracker.AutoDetectChangesEnabled);
    coach.Name = "Alex Petrescu";
    context.ChangeTracker.DetectChanges();
    Console.WriteLine(context.ChangeTracker.DebugView.LongView);

    await context.SaveChangesAsync();
    Console.WriteLine(context.ChangeTracker.DebugView.LongView);
}

async Task UpdateWithNoTracking()
{
    var coach1 = await context.Coaches
     .AsNoTracking()
     .FirstOrDefaultAsync(t => t.Id == 9);
    coach1.Name = "Testing Tracking Behaviour";
    Console.WriteLine(context.ChangeTracker.DebugView.LongView);

    context.Update(coach1);
    Console.WriteLine(context.ChangeTracker.DebugView.LongView);

    await context.SaveChangesAsync();
    Console.WriteLine(context.ChangeTracker.DebugView.LongView);
}

async Task Delete()
{
    var coach = await context.Coaches.FindAsync(9);
    Console.WriteLine(context.ChangeTracker.DebugView.LongView);

    context.Remove(coach);
    Console.WriteLine(context.ChangeTracker.DebugView.LongView);

    await context.SaveChangesAsync();
    Console.WriteLine(context.ChangeTracker.DebugView.LongView);
}

async Task ExecuteDelete()
{
    // Usual example
    //var coaches = await context.Coaches.Where( t => t.Name == "Gigi Becali").ToListAsync();
    //context.RemoveRange(coaches);
    //context.SaveChangesAsync();

    // Execute directly on db, isnt waiting until SaveChanges()
    var coaches1 = await context.Coaches.Where(t => t.Name == "Gigi Becali").ExecuteDeleteAsync();
}

async Task ExecuteUpdate()
{
    await context.Coaches.Where(t => t.Name == "Gigi Becali").ExecuteUpdateAsync(set => set
    .SetProperty(prop => prop.Name, "Vasile Bragadiru")
    .SetProperty(prop => prop.CreatedDate, DateTime.Now));
}

#endregion

#region Related Data
// Insert record with FK
// await InsertMatch();

// Insert Parent/Child
// await InsertTeamWithCoach();

// Insert Parent with Children
// await InserLeagueWithTeams();

// Eager Loading -  Including Related Data 
//await GetLeagueEagerLoading();

// Explicit Loading - Including Related Data
// await GetLeagueExplicitLoading();

// Lazy Loading - Including Related Data
// await GetLeagueLazyLoading();

// Filtering Includes
// Get all teams and only home matches they have scored
//await InsertMoreMatches();
// await FilteringIncludes();

// Projections and Annonymous types
var teams = await context.Teams
    .Select( q => new TeamDetails
    {
        TeamId = q.Id,
        TeamName = q.Name,
        CoachName = q.Coach.Name,
        TotalHomeGoals = q.HomeMatches.Sum(q => q.HomeTeamScore),
        TotalAwayGoals = q.HomeMatches.Sum(q => q.AwayTeamScore),
    })
    .ToListAsync();

foreach (var team in teams)
{
    Console.WriteLine($"{team.TeamName} - {team.CoachName} | Home Goals: {team.TotalHomeGoals}  Away Goals: {team.TotalAwayGoals}");
}

async Task InsertMatch()
{
    var match = new Match
    {
        AwayTeamId = 1,
        HomeTeamId = 2,
        HomeTeamScore = 0,
        AwayTeamScore = 0,
        Date = new DateTime(2025, 10, 1),
        TicketPrice = 20
    };

    await context.Matches.AddAsync(match);
    await context.SaveChangesAsync();

    // Incorrect reference data -> wil give error
    //var match1 = new Match
    //{
    //    AwayTeamId = 10,
    //    HomeTeamId = 0,
    //    HomeTeamScore = 0,
    //    AwayTeamScore = 0,
    //    Date = new DateTime(2025, 10, 1),
    //    TicketPrice = 20
    //};

    //await context.Matches.AddAsync(match);
    //await context.SaveChangesAsync();
}

async Task InsertTeamWithCoach()
{
    var team = new Team
    {
        Name = "New Team",
        Coach = new Coach
        {
            Name = "Trevoir Wiilliam"
        }
    };

    await context.Teams.AddAsync(team);
    await context.SaveChangesAsync();
}

async Task InserLeagueWithTeams()
{
    var league = new League
    {
        Name = "New League",
        Teams = new List<Team>
        {
            new Team
            {
                Name = "Juvents",
                Coach = new Coach
                {
                    Name = "Ionica"
                },
            },
            new Team
            {
                Name = "AC Milan",
                Coach = new Coach
                {
                    Name = "Milanica"
                },
            },
            new Team
            {
                Name = "Zaaaz",
                Coach = new Coach
                {
                    Name = "Zaza"
                },
            },
        }
    };

    await context.Leagues.AddAsync(league);
    await context.SaveChangesAsync();
}

async Task GetLeagueRelatedData()
{
    var leagues = await context.Leagues
    .Include(q => q.Teams)
        .ThenInclude(q => q.Coach)
    .ToListAsync();

    foreach (var league in leagues)
    {
        Console.WriteLine("league " + league.Name);
        foreach (var team in league.Teams)
        {
            Console.WriteLine("team " + team.Name + " - coach " + team.Coach.Name);
        }
    }
}

async Task GetLeagueExplicitLoading()
{
    var league = await context.FindAsync<League>(1); // Find() hasn t loaded related data

    if (!league.Teams.Any())                         // Any() hasn t loaded related data
    {
        Console.WriteLine("Teams have not been loaded");
    }

    await context.Entry(league)
        .Collection(q => q.Teams)
        .LoadAsync();          // loads related data 

    if (league.Teams.Any())
    {
        foreach (var team in league.Teams)
        {
            Console.WriteLine($"{team.Name}");
        }
    }
}

async Task GetLeagueLazyLoading()
{
    //var league = await context.FindAsync<League>(1);

    //foreach (var team in league.Teams)
    //{
    //        Console.WriteLine(team.Name);
    //}

    foreach (var league in context.Leagues) 
    {
        foreach (var team in league.Teams) 
        {
            Console.WriteLine($"{team.Name} - {team.Coach.Name}"); 
        }
    } 
}

async Task InsertMoreMatches()
{
    var match1 = new Match
    {
        AwayTeamId = 2,
        HomeTeamId = 3,
        HomeTeamScore = 1,
        AwayTeamScore = 0,
        Date = new DateTime(2023, 01, 01),
        TicketPrice = 20,
    };

    var match2 = new Match
    {
        AwayTeamId = 2,
        HomeTeamId = 1,
        HomeTeamScore = 1,
        AwayTeamScore = 0,
        Date = new DateTime(2023, 01, 01),
        TicketPrice = 20,
    };

    var match3 = new Match
    {
        AwayTeamId = 1,
        HomeTeamId = 3,
        HomeTeamScore = 1,
        AwayTeamScore = 0,
        Date = new DateTime(2023, 01, 01),
        TicketPrice = 20,
    };

    var match4 = new Match
    {
        AwayTeamId = 4,
        HomeTeamId = 3,
        HomeTeamScore = 0,
        AwayTeamScore = 1,
        Date = new DateTime(2023, 01, 01),
        TicketPrice = 20,
    };

    await context.AddRangeAsync(match1, match2, match3, match4);
    await context.SaveChangesAsync();
}

async Task FilteringIncludes()
{

    var teams = await context.Teams
    .Include("Coach")
    .Include(q => q.HomeMatches.Where(q => q.HomeTeamScore > 0))
    .ToListAsync();

    foreach (var team in teams)
    {
        Console.WriteLine($"{team.Name} - {team.Coach.Name}");
        foreach (var match in team.HomeMatches)
        {
            Console.WriteLine($"Score = {match.HomeTeamScore}");
        }
    }
}
#endregion
class TeamInfo
{
    public int TeamId { get; set; }
    public string Name { get; set; }
}

class TeamDetails
{
    public int TeamId { get; set; }
    public string TeamName { get; set; }
    public string CoachName { get; set; }

    public int TotalHomeGoals { get; set; }
    public int TotalAwayGoals {  get; set; }
}

