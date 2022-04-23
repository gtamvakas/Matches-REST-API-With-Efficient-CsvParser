using System.Net.Http.Headers;
using System.Runtime.Caching;
using cw2backend.CacheLayer;
using cw2backend.Models;
using Npgsql;

namespace cw2backend.Repositories;

public class ProxyMatchesRepository : IMatchesRepository
{
    //private IMatchesRepository _matchesRepository = new MatchesRepository();
    private readonly NpgsqlConnection _dbConnection;
    private readonly ICacheManager _cacheManager;

    public ProxyMatchesRepository(NpgsqlConnection dbConnection)
    {
        _dbConnection = dbConnection;
        _cacheManager = new CacheManager("ModifiersCache");
    }
    
    
    
    
    
    private async Task<ModifiersJsonResponse> GetActiveModifiersAsync()
    {
        var client = new HttpClient();
        //HARD CODED BEARER TOKEN WOOHOOOOOOOOOOOOOOOOOOOOOOOOOOOO
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
        var teamsList = new List<string>();
        ModifiersJsonResponse modifiersList = null;
        _dbConnection.Open();
        
        if (_cacheManager.Get<ModifiersJsonResponse>("modifier") == null)
        {
            var task = Task.Run(GetActiveModifiersAsync);
            task.Wait();
            modifiersList = task.Result;
            CacheItemPolicy policy = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(30)
            };
            _cacheManager.Set("modifier", modifiersList, policy );
        }
        else
        {
            modifiersList = _cacheManager.Get<ModifiersJsonResponse>("modifiers");
        }
        string query =
            @"SELECT DISTINCT home_team, away_team FROM exercise.matches WHERE division IN (@division, @division2)";
        using (var cmd = new NpgsqlCommand(query, _dbConnection))
        {
            //I know it's absolutely retarded but I can't do it with a loop because my brain hurts from absence of sleep.
            cmd.Parameters.AddWithValue("division", modifiersList.Modifiers[0].Division);
            cmd.Parameters.AddWithValue("division2", modifiersList.Modifiers[1].Division);
                
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
        var matchesList = new List<Match>();
        ModifiersJsonResponse modifiersList = null;
        _dbConnection.Open();
        
        if (_cacheManager.Get<ModifiersJsonResponse>("modifier") == null)
        {
            var task = Task.Run(GetActiveModifiersAsync);
            task.Wait();
            modifiersList = task.Result;
            CacheItemPolicy policy = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(30)
            };
            _cacheManager.Set("modifier", modifiersList, policy );
        }
        else
        {
            modifiersList = _cacheManager.Get<ModifiersJsonResponse>("modifiers");
        }
        string query = "SELECT * FROM exercise.matches WHERE date=@date AND division IN (@division, @division2)";
        using (var cmd = new NpgsqlCommand(query, _dbConnection))
        {
            cmd.Parameters.AddWithValue("division", modifiersList.Modifiers[0].Division);
            cmd.Parameters.AddWithValue("division2", modifiersList.Modifiers[1].Division);
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
        ModifiersJsonResponse modifiersList = null;
        var matchesList = new List<Match>();
        _dbConnection.Open(); 
        if (_cacheManager.Get<ModifiersJsonResponse>("modifier") == null)
        {
            var task = Task.Run(GetActiveModifiersAsync);
            task.Wait();
            modifiersList = task.Result;
            CacheItemPolicy policy = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(30)
            };
            _cacheManager.Set("modifier", modifiersList, policy );
        }
        else
        {
            modifiersList = _cacheManager.Get<ModifiersJsonResponse>("modifiers");
        }
        string query = "SELECT * FROM exercise.matches WHERE (home_team=@teamName OR away_team=@teamName) AND division IN (@division, @division2)";
        using (var cmd = new NpgsqlCommand(query, _dbConnection))
        {
                
            cmd.Parameters.AddWithValue("teamName", teamName);
               
            cmd.Parameters.AddWithValue("division", modifiersList.Modifiers[0].Division);
            cmd.Parameters.AddWithValue("division2", modifiersList.Modifiers[1].Division);


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
        ModifiersJsonResponse modifiersList = null;
        var matchesList = new List<Match>();
        _dbConnection.Open();
        if (_cacheManager.Get<ModifiersJsonResponse>("modifier") == null)
        {
            var task = Task.Run(GetActiveModifiersAsync);
            task.Wait();
            modifiersList = task.Result;
            CacheItemPolicy policy = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(30)
            };
            _cacheManager.Set("modifier", modifiersList, policy );
        }
        else
        {
            modifiersList = _cacheManager.Get<ModifiersJsonResponse>("modifiers");
        }
        string query = "SELECT * FROM exercise.matches WHERE date BETWEEN @from AND @to AND division IN (@division, @division2)";
        using (var cmd = new NpgsqlCommand(query, _dbConnection))
        {
            
            cmd.Parameters.AddWithValue("division", modifiersList.Modifiers[0].Division);
            cmd.Parameters.AddWithValue("division2", modifiersList.Modifiers[1].Division);
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

    public List<Modifier> GetModifiers()
    {
        ModifiersJsonResponse modifiersList = null;
        if (_cacheManager.Get<ModifiersJsonResponse>("modifier") == null)
        {
            var task = Task.Run(GetActiveModifiersAsync);
            task.Wait();
            modifiersList = task.Result;
            CacheItemPolicy policy = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(30)
            };
            _cacheManager.Set("modifier", modifiersList, policy );
        }
        else
        {
            modifiersList = _cacheManager.Get<ModifiersJsonResponse>("modifiers");
        }

        return modifiersList.Modifiers;
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

    
}