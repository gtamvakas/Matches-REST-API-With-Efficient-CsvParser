using cw2backend.Models;
using PostgreSQLCopyHelper;

namespace cw2backend.Helpers;

public class PgsqlCopyHelperWrapper
{
    public static PostgreSQLCopyHelper<Match> CreateMatchCopyHelper()
    {
        var copyHelper = new PostgreSQLCopyHelper<Match>("exercise", "matches")
            .MapBigInt("id", x => x.Id)
            .MapText("division", x => x.Division)
            .MapDate("date", x => x.Date)
            .MapText("home_team", x => x.HomeTeam)
            .MapText("away_team", x => x.AwayTeam)
            .MapSmallInt("ft_home_goals", x => x.FtHomeGoals)
            .MapSmallInt("ft_away_goals", x => x.FtAwayGoals)
            .MapSmallInt("ht_home_goals", x => x.HtHomeGoals)
            .MapSmallInt("ht_away_goals", x => x.HtAwayGoals)
            .MapText("full_time_result", x => x.FullTimeResult.ToString())
            .MapText("half_time_result", x => x.HalfTimeResult.ToString());
        return copyHelper;
    }

    public static PostgreSQLCopyHelper<Match> CreateStatsCopyHelper()
    {
        var copyHelper = new PostgreSQLCopyHelper<Match>("exercise", "stats")
            .MapBigInt("match_id", x => x.Id)
            .MapInteger("attendance", x => x.Attendance)
            .MapText("referee", x => x.Referee);
        return copyHelper;
    }
}