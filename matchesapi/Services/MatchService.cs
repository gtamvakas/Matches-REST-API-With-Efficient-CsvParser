using System.Collections.Concurrent;
using cw2backend.Helpers;
using cw2backend.Models;
using cw2backend.Repositories;
using Npgsql;

namespace cw2backend.Services;

public class MatchService : IMatchService
{
    private readonly IMatchesRepository _repository;
    private IMatchService _matchServiceImplementation;


    public MatchService(IMatchesRepository repository)
    {
        _repository = repository;
    }


    private bool CsvDataAlreadyParsed()
    {
        using var dbConnection = new NpgsqlConnection("Host=127.0.0.1;Username=postgres;Password=password;Database=matchstats;");
        dbConnection.Open();
        string query = @"SELECT EXISTS(SELECT id FROM exercise.matches) AS exists";
        using var cmd = new NpgsqlCommand(query, dbConnection);
        var dbResult = cmd.ExecuteReader();
        while (dbResult.Read())
        {
            if (Convert.ToBoolean(dbResult["exists"]))
            {
                return true;
            }
        }

        return false;
    }

    public bool WriteFromCsvToDb(BlockingCollection<Match> matches)
    {
        if (CsvDataAlreadyParsed())
        {
            return false;
        }
        using var dbConnection = new NpgsqlConnection("Host=127.0.0.1;Username=postgres;Password=password;Database=matchstats;");
        dbConnection.Open();
        var matchCopyHelper = PgsqlCopyHelperWrapper.CreateMatchCopyHelper();
        var statsCopyHelper = PgsqlCopyHelperWrapper.CreateStatsCopyHelper();
        matchCopyHelper.SaveAll(dbConnection, matches);
        statsCopyHelper.SaveAll(dbConnection, matches);
        dbConnection.Close();

        return true;
    }
     
    public IEnumerable<string> GetTeamsByDivision(string division, bool useModifiers = false)
    {
        return _repository.GetTeamsByDivision(division, useModifiers);
         
    }

    public IEnumerable<Match> GetMatchesByDate(DateTime date, bool useModifiers = false)
    {
        return _repository.GetMatchesByDate(date, useModifiers);
    }

    public IEnumerable<Match> GetMatchesByTeam(string teamName, bool useModifiers = false)
    {
        return _repository.GetMatchesByTeam(teamName, useModifiers);
    }

    public IEnumerable<Match> GetMatchesByTimePeriod(DateTime from, DateTime to, bool useModifiers = false)
    {
        return _repository.GetMatchesByTimePeriod(from, to, useModifiers);
    }

    public int InsertMatch(Match match)
    {
        return _repository.InsertMatch(match);
    }

    public int InsertStatistic(Match match)
    {
        return _repository.InsertStatistic(match);
    }

    public int EditMatch(int id, Match match)
    {
        return _repository.EditMatch(id, match);
    }

    public int EditStatistic(int id, Match match)
    {
        return _repository.EditStatistic(id, match);
    }

    public int DeleteMatch(int id)
    {
        return _repository.DeleteMatch(id);
;    }

    public int DeleteAllData()
    {
        return _repository.DeleteAllData();
    }

    public List<Modifier> GetModifiers()
    {
        return _repository.GetModifiers();
    }
}