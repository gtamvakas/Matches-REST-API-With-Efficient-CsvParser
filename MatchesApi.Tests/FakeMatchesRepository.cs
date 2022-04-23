using System;
using System.Collections.Generic;
using System.Linq;
using cw2backend.Models;
using cw2backend.Repositories;

namespace MatchesApi.Tests;

public class FakeMatchesRepository : IMatchesRepository
{
    private readonly IEnumerable<Match> _matchesListAsDatabase;

    public FakeMatchesRepository()
    {
        _matchesListAsDatabase = new List<Match>()
        {
            new Match()
            {
              Division  = "E0",
              HomeTeam = "Arsenal",
              AwayTeam = "Manchester",
              HtHomeGoals = 4,
              HtAwayGoals = 3,
              FtHomeGoals = 4,
              FtAwayGoals = 4,
              FullTimeResult = 'W',
              HalfTimeResult = 'D',
              Attendance = 3333,
              Referee = "John"
            },
            new Match()
            {
                Division  = "E1",
                HomeTeam = "Totenham",
                AwayTeam = "Real Madrid",
                HtHomeGoals = 2,
                HtAwayGoals = 4,
                FtHomeGoals = 2,
                FtAwayGoals = 4,
                FullTimeResult = 'W',
                HalfTimeResult = 'D',
                Attendance = 4444,
                Referee = "Thomas"
            },
            new Match()
            {
                Division  = "EC",
                HomeTeam = "Bayern",
                AwayTeam = "Barcelona",
                HtHomeGoals = 1,
                HtAwayGoals = 1,
                FtHomeGoals = 1,
                FtAwayGoals = 1,
                FullTimeResult = 'W',
                HalfTimeResult = 'D',
                Attendance = 5555,
                Referee = "George"
            },
        };
    }   


    IEnumerable<string> GetTeamsByDivision(string division, bool useModifiers = false)
    {
        var result = new List<string>();
        foreach (var match in _matchesListAsDatabase)
        {
            if (match.Division.Equals(division))
            {
                result.Add(match.AwayTeam);
                result.Add(match.HomeTeam);
            }
        }

        return result;
    }

    public IEnumerable<Match> GetMatchesByDate(DateTime date, bool useModifiers = false)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Match> GetMatchesByTeam(string teamName, bool useModifiers = false)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Match> GetMatchesByTimePeriod(DateTime @from, DateTime to, bool useModifiers = false)
    {
        throw new NotImplementedException();
    }

    public int InsertMatch(Match match)
    {
        throw new NotImplementedException();
    }

    public int InsertStatistic(Match match)
    {
        throw new NotImplementedException();
    }

    public int EditMatch(int id, Match match)
    {
        throw new NotImplementedException();
    }

    public int EditStatistic(int id, Match match)
    {
        throw new NotImplementedException();
    }

    public int DeleteMatch(int id)
    {
        throw new NotImplementedException();
    }

    public int DeleteAllData()
    {
        throw new NotImplementedException();
    }

    public List<Modifier> GetModifiers()
    {
        throw new NotImplementedException();
    }

    // IEnumerable<Match> GetMatchesByDate(DateTime date, bool useModifiers = false);
    // IEnumerable<Match> GetMatchesByTeam(string teamName, bool useModifiers = false);
    // IEnumerable<Match> GetMatchesByTimePeriod(DateTime from, DateTime to, bool useModifiers = false);
    // int InsertMatch(Match match);
    // int InsertStatistic(Match match);
    // int EditMatch(int id, Match match);
    // int EditStatistic(int id, Match match);
    // int DeleteMatch(int id);
    // int DeleteAllData();
    IEnumerable<string> IMatchesRepository.GetTeamsByDivision(string division, bool useModifiers)
    {
        return GetTeamsByDivision(division, useModifiers);
    }
}