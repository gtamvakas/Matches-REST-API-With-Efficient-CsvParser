using cw2backend.Models;

namespace cw2backend.Repositories;

public interface IMatchesRepository
{
    
    IEnumerable<string> GetTeamsByDivision(string division, bool useModifiers = false);
    IEnumerable<Match> GetMatchesByDate(DateTime date, bool useModifiers = false);
    IEnumerable<Match> GetMatchesByTeam(string teamName, bool useModifiers = false);
    IEnumerable<Match> GetMatchesByTimePeriod(DateTime from, DateTime to, bool useModifiers = false);
    int InsertMatch(Match match);
    int InsertStatistic(Match match);
    int EditMatch(int id, Match match);
    int EditStatistic(int id, Match match);
    int DeleteMatch(int id);
    int DeleteAllData();

    List<Modifier> GetModifiers();


}