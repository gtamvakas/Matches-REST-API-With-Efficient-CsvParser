using System.Net.Http.Headers;
using System.Runtime.Caching;
using System.Text.Json;
using cw2backend.CacheLayer;
using cw2backend.Helpers;
using cw2backend.Models;
using Microsoft.VisualBasic;
using Npgsql;
using System.Collections.Generic;
namespace cw2backend.Repositories;

public class MatchesRepository : IMatchesRepository
{
    private NpgsqlConnection _dbConnection;
    //private ICacheManager _cacheManager;
    private IMatchesRepository _proxyMatchesRepository;
    public MatchesRepository()
    {
        _dbConnection = new NpgsqlConnection("Host=127.0.0.1;Username=postgres;Password=password;Database=matchstats;");
        //_cacheManager = new CacheManager("ModifiersCache");
        _proxyMatchesRepository = new ProxyMatchesRepository(_dbConnection);

    }


    private async Task<ModifiersJsonResponse> GetActiveModifiersAsync()
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJodHRwczovL2xvY2FsaG9zdDo1MDAxIiwiYXVkIjoiaHR0cDovL2xvY2FsaG9zdDo1MDAwIiwicm9sZXMiOiJEZXZlbG9wZXIiLCJleHAiOjE2NjM2ODY4Nzh9.ANe1bsE4SALD98aH_YDsC90At-iJ4lFV-CiNKQ2q_40");
        
      
        // var streamResult = await client.GetStreamAsync("https://be-2021-cw2-external-api.herokuapp.com/v1/modifiers/active");
        //
        // var jsonResponse = JsonSerializer.DeserializeAsync<ModifiersJsonResponse>(streamResult);
        HttpResponseMessage response =
            await client.GetAsync("https://be-2021-cw2-external-api.herokuapp.com/v1/modifiers/active");
        var jsonResponse = await response.Content.ReadFromJsonAsync<ModifiersJsonResponse>();

        return  jsonResponse;
    }  
    
    public IEnumerable<string> GetTeamsByDivision(string division, bool useModifiers = false)
    {
       
        if (useModifiers)
        {
            return _proxyMatchesRepository.GetTeamsByDivision(division, true);
        }
        var teamsList = new List<string>();

        string query = "SELECT DISTINCT home_team, away_team FROM exercise.matches WHERE division=@division";
        _dbConnection.Open();
        using (var cmd = new NpgsqlCommand(query, _dbConnection))
        {
            cmd.Parameters.AddWithValue("division", division);

            var dbResult = cmd.ExecuteReader();
            while (dbResult.Read())
            {
                teamsList.Add(dbResult["home_team"].ToString()??"");
                teamsList.Add(dbResult["away_team"].ToString()??"");
            }
        }
        _dbConnection.Close();
        
        return teamsList;
    }

    public IEnumerable<Match> GetMatchesByDate(DateTime date, bool useModifiers = false)
    {
        if (useModifiers)
        {
            return _proxyMatchesRepository.GetMatchesByDate(date, true);
        }
        var matchesList = new List<Match>();
        _dbConnection.Open();
        string query = @"SELECT * FROM exercise.matches WHERE date=@date";
        
        using (var cmd = new NpgsqlCommand(query, _dbConnection))
        {
            cmd.Parameters.AddWithValue("date", date);

            var dbResult = cmd.ExecuteReader();
            while (dbResult.Read())
            {
                var match = new Match()
                {
                    Id = Convert.ToInt64(dbResult["id"]),
                    Division = Convert.ToString(dbResult["division"])??null!,
                    Date = string.IsNullOrEmpty(Convert.ToString(dbResult["date"])) ? null : Convert.ToDateTime(dbResult["date"]) ,
                    HomeTeam = Convert.ToString(dbResult["home_team"])!,
                    AwayTeam = Convert.ToString(dbResult["away_team"])!,
                    FtHomeGoals = string.IsNullOrEmpty(Convert.ToString(dbResult["ft_home_goals"])) ? null : Convert.ToInt16(dbResult["ft_home_goals"]),
                    FtAwayGoals = string.IsNullOrEmpty(Convert.ToString(dbResult["ft_away_goals"])) ? null : Convert.ToInt16(dbResult["ft_away_goals"]),
                    HtHomeGoals = string.IsNullOrEmpty(Convert.ToString(dbResult["ht_home_goals"])) ? null : Convert.ToInt16(dbResult["ht_home_goals"]),
                    HtAwayGoals = string.IsNullOrEmpty(Convert.ToString(dbResult["ht_away_goals"])) ? null : Convert.ToInt16(dbResult["ht_away_goals"]),
                    FullTimeResult = Convert.ToChar(dbResult["full_time_result"]),
                    HalfTimeResult = string.IsNullOrEmpty(Convert.ToString(dbResult["half_time_result"])) ? null : Convert.ToChar(dbResult["half_time_result"]) ,
                    
                    
                };
                matchesList.Add(match);
            }
        }
        _dbConnection.Close();
        
        return matchesList;
    }

    public IEnumerable<Match> GetMatchesByTeam(string teamName, bool useModifiers = false)
    {

        if (useModifiers)
        {
            return _proxyMatchesRepository.GetMatchesByTeam(teamName, true);
        }
       
        var matchesList = new List<Match>();
        _dbConnection.Open();
        string query = @"SELECT *  FROM exercise.matches WHERE home_team = @teamName OR away_team=@teamName";
 
        using (var cmd = new NpgsqlCommand(query, _dbConnection))
        {
            cmd.Parameters.AddWithValue("teamName", teamName);

            var dbResult = cmd.ExecuteReader();
            while (dbResult.Read())
            {
                var match = new Match()
                {
                    Id = Convert.ToInt64(dbResult["id"]),
                    Division = Convert.ToString(dbResult["division"])??null!,
                    Date = string.IsNullOrEmpty(Convert.ToString(dbResult["date"])) ? null : Convert.ToDateTime(dbResult["date"]) ,
                    HomeTeam = Convert.ToString(dbResult["home_team"])!,
                    AwayTeam = Convert.ToString(dbResult["away_team"])!,
                    FtHomeGoals = string.IsNullOrEmpty(Convert.ToString(dbResult["ft_home_goals"])) ? null : Convert.ToInt16(dbResult["ft_home_goals"]),
                    FtAwayGoals = string.IsNullOrEmpty(Convert.ToString(dbResult["ft_away_goals"])) ? null : Convert.ToInt16(dbResult["ft_away_goals"]),
                    HtHomeGoals = string.IsNullOrEmpty(Convert.ToString(dbResult["ht_home_goals"])) ? null : Convert.ToInt16(dbResult["ht_home_goals"]),
                    HtAwayGoals = string.IsNullOrEmpty(Convert.ToString(dbResult["ht_away_goals"])) ? null : Convert.ToInt16(dbResult["ht_away_goals"]),
                    FullTimeResult = Convert.ToChar(dbResult["full_time_result"]),
                    HalfTimeResult = string.IsNullOrEmpty(Convert.ToString(dbResult["half_time_result"])) ? null : Convert.ToChar(dbResult["half_time_result"]) ,
                    
                    
                };
                matchesList.Add(match);
            }
        }
        _dbConnection.Close();
        
        return matchesList;
    }

    public IEnumerable<Match> GetMatchesByTimePeriod(DateTime from, DateTime to, bool useModifiers = false)
    {
        if (useModifiers)
        {
            return _proxyMatchesRepository.GetMatchesByTimePeriod(from, to, true);
        }     
        var matchesList = new List<Match>();
        _dbConnection.Open();
        string query = @"SELECT *  FROM exercise.matches WHERE date BETWEEN @from AND @to";
        using (var cmd = new NpgsqlCommand(query, _dbConnection))
        {
            cmd.Parameters.AddWithValue("from", from);
            cmd.Parameters.AddWithValue("to", to);
            var dbResult = cmd.ExecuteReader();
            while (dbResult.Read())
            {
                var match = new Match()
                {
                    Id = Convert.ToInt64(dbResult["id"]),
                    Division = Convert.ToString(dbResult["division"])??null!,
                    Date = string.IsNullOrEmpty(Convert.ToString(dbResult["date"])) ? null : Convert.ToDateTime(dbResult["date"]) ,
                    HomeTeam = Convert.ToString(dbResult["home_team"])!,
                    AwayTeam = Convert.ToString(dbResult["away_team"])!,
                    FtHomeGoals = string.IsNullOrEmpty(Convert.ToString(dbResult["ft_home_goals"])) ? null : Convert.ToInt16(dbResult["ft_home_goals"]),
                    FtAwayGoals = string.IsNullOrEmpty(Convert.ToString(dbResult["ft_away_goals"])) ? null : Convert.ToInt16(dbResult["ft_away_goals"]),
                    HtHomeGoals = string.IsNullOrEmpty(Convert.ToString(dbResult["ht_home_goals"])) ? null : Convert.ToInt16(dbResult["ht_home_goals"]),
                    HtAwayGoals = string.IsNullOrEmpty(Convert.ToString(dbResult["ht_away_goals"])) ? null : Convert.ToInt16(dbResult["ht_away_goals"]),
                    FullTimeResult = Convert.ToChar(dbResult["full_time_result"]),
                    HalfTimeResult = string.IsNullOrEmpty(Convert.ToString(dbResult["half_time_result"])) ? null : Convert.ToChar(dbResult["half_time_result"]) ,
                    
                    
                };
                matchesList.Add(match);
            }
        }
        _dbConnection.Close();
        
        return matchesList;
    }

    public int InsertMatch(Match match)
    { 
            string query = @"
            INSERT INTO exercise.matches(id, division, date, home_team, away_team, ft_home_goals, ft_away_goals, ht_home_goals, ht_away_goals, full_time_result, half_time_result) 
            VALUES (@id, @division, @date, @home_team, @away_team, @ft_home_goals, @ft_away_goals, @ht_home_goals, @ht_away_goals, @full_time_result, @half_time_result)";
            _dbConnection.Open();
        int queryResultCode;
        using (var cmd = new NpgsqlCommand(query, _dbConnection))
        {
            cmd.Parameters.AddWithValue("id", match.Id);
            cmd.Parameters.AddWithValue("division", match.Division);
            cmd.Parameters.AddWithValue("date", match.Date ?? (object) DBNull.Value);
            cmd.Parameters.AddWithValue("home_team", match.HomeTeam);
            cmd.Parameters.AddWithValue("away_team", match.AwayTeam);
            cmd.Parameters.AddWithValue("ft_home_goals", match.FtHomeGoals ?? 0);
            cmd.Parameters.AddWithValue("ft_away_goals", match.FtAwayGoals ?? 0);
            cmd.Parameters.AddWithValue("ht_home_goals", match.HtHomeGoals ?? 0);
            cmd.Parameters.AddWithValue("ht_away_goals", match.HtAwayGoals ?? 0);
            cmd.Parameters.AddWithValue("full_time_result", match.FullTimeResult);
            cmd.Parameters.AddWithValue("half_time_result", match.HalfTimeResult ?? (object) DBNull.Value);
            queryResultCode = cmd.ExecuteNonQuery();
        }
        _dbConnection.Close();
        return queryResultCode;
    }

    public int InsertStatistic(Match match)
    {
        string query = @"
            INSERT INTO exercise.stats(match_id, attendance, referee) 
            VALUES (@match_id, @attendance, @referee)";
        _dbConnection.Open();
        int queryResultCode;
        using var cmd = new NpgsqlCommand(query, _dbConnection);
        cmd.Parameters.AddWithValue("match_id", match.Id);
        cmd.Parameters.AddWithValue("attendance", match.Attendance ?? 0);
        cmd.Parameters.AddWithValue("referee", match.Referee ?? (object) DBNull.Value);
        queryResultCode = cmd.ExecuteNonQuery();
        _dbConnection.Close();
        return queryResultCode;
    }

    public int EditMatch(int id, Match match)
    {
         string query = @"
            UPDATE exercise.matches
            SET division = @division, date = @date, home_team = @home_team, away_team = @away_team, ft_home_goals = @ft_home_goals,
            ft_away_goals = @ft_away_goals, ht_home_goals = @ht_home_goals, ht_away_goals = @ht_away_goals, 
            full_time_result = @full_time_result, half_time_result = @half_time_result
            WHERE id = @id";
         _dbConnection.Open();
        int queryResultCode;
        using (var cmd = new NpgsqlCommand(query, _dbConnection))
        {
            cmd.Parameters.AddWithValue("id", id);
            cmd.Parameters.AddWithValue("division", match.Division);
            cmd.Parameters.AddWithValue("date", match.Date ?? (object) DBNull.Value);
            cmd.Parameters.AddWithValue("home_team", match.HomeTeam);
            cmd.Parameters.AddWithValue("away_team", match.AwayTeam);
            cmd.Parameters.AddWithValue("ft_home_goals", match.FtHomeGoals ?? 0);
            cmd.Parameters.AddWithValue("ft_away_goals", match.FtAwayGoals ?? 0);
            cmd.Parameters.AddWithValue("ht_home_goals", match.HtHomeGoals ?? 0);
            cmd.Parameters.AddWithValue("ht_away_goals", match.HtAwayGoals ?? 0);
            cmd.Parameters.AddWithValue("full_time_result", match.FullTimeResult);
            cmd.Parameters.AddWithValue("half_time_result", match.HalfTimeResult ?? (object) DBNull.Value);
            queryResultCode = cmd.ExecuteNonQuery();
        }
        _dbConnection.Close();
        return queryResultCode;
    }

    public int EditStatistic(int id, Match match)
    {
        string query = @"
            UPDATE exercise.stats
            SET attendance = @attendance, referee = @referee
            WHERE match_id = @match_id";
        _dbConnection.Open();
        int queryResultCode;
        using (var cmd = new NpgsqlCommand(query, _dbConnection))
        {
            cmd.Parameters.AddWithValue("match_id", id);
            cmd.Parameters.AddWithValue("attendance", match.Attendance ?? 0);
            cmd.Parameters.AddWithValue("referee", match.Referee ?? (object) DBNull.Value);
            queryResultCode = cmd.ExecuteNonQuery();
        }
        _dbConnection.Close();
        return queryResultCode;
    }

    public int DeleteMatch(int id)
    {
        string query = @"
           DELETE FROM exercise.matches
           WHERE id=@id";
        _dbConnection.Open();
        int queryResultCode;
        using (var cmd = new NpgsqlCommand(query, _dbConnection))
        {
            cmd.Parameters.AddWithValue("id", id);
            queryResultCode = cmd.ExecuteNonQuery();
        }
        _dbConnection.Close();
        return queryResultCode;
    }

    public int DeleteAllData()
    {
        string query = @"
           DELETE FROM exercise.matches CASCADE";
        _dbConnection.Open();
        int queryResultCode;
        using (var cmd = new NpgsqlCommand(query, _dbConnection))
        {
            queryResultCode = cmd.ExecuteNonQuery();
        }
        _dbConnection.Close();
        return queryResultCode;
    }

    public List<Modifier> GetModifiers()
    {
        return _proxyMatchesRepository.GetModifiers();
    }
}