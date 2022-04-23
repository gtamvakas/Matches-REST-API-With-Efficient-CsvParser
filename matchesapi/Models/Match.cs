namespace cw2backend.Models;

public class Match
{
    

    public Match()
    {
        Id = Interlocked.Increment(ref _matchId);
    }
    
    private static long _matchId;

    public long Id { get; set; } 
    public string Division { get; set; }
    public DateTime? Date { get; set; }
    public string HomeTeam { get; set; }
    public string AwayTeam{ get; set; }
    public short? FtHomeGoals{ get; set; }
    public short? FtAwayGoals{ get; set; }
    public char FullTimeResult{ get; set; }
    public short? HtHomeGoals{ get; set; }
    public short? HtAwayGoals{ get; set; }
    public char? HalfTimeResult{ get; set; }
    public int? Attendance{ get; set; }
    public string? Referee { get; set; }

    public override string ToString()
    {
        return
            $"{Id}, {Division}, {Date}, {HomeTeam}, {AwayTeam}, {FtHomeGoals}, {FtAwayGoals}, {FullTimeResult}, {HtHomeGoals}, {HtAwayGoals}, {HalfTimeResult}, {Attendance}, {Referee}";
    }

    public bool Valid()
    {
        return !string.IsNullOrEmpty(Division) &&
               !string.IsNullOrEmpty(HomeTeam) &&
               !string.IsNullOrEmpty(AwayTeam) &&
               !string.IsNullOrEmpty(FullTimeResult.ToString());
    }
}