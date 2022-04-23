using System.Collections.Concurrent;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using cw2backend.Models;

namespace cw2backend.Helpers;

public class CsvHelperWrapper
{
    private string _csvPath;
    private BlockingCollection<Match> _matches;
    public CsvHelperWrapper(string csvPath, BlockingCollection<Match> matches)
    {
        _csvPath = csvPath;
        _matches = matches;
    }

    public void ReadCsv()
    {
       
        using (var csv = new CsvReader(new StreamReader(_csvPath), GetConfig()))
        {
            csv.Read();
            csv.ReadHeader();
            while (csv.Read())
            {
                var match = new Match()
                {
                    Division = Convert.ToString(csv.GetField("Div")),
                    Date = DateTime.ParseExact(csv.GetField("Date"), new []{"dd/MM/yyyy", "dd/MM/yy", "d/M/yy", "d/M/yyyy"}, null),  
                    HomeTeam = Convert.ToString(csv.GetField("HomeTeam")),
                    AwayTeam = Convert.ToString(csv.GetField("AwayTeam")),
                    FtHomeGoals = Convert.ToInt16(csv.GetField("FTHG")??"0"),
                    FtAwayGoals = Convert.ToInt16(csv.GetField("FTAG")??"0"),
                    HtHomeGoals = Convert.ToInt16(csv.GetField("HTHG")??"0"),
                    HtAwayGoals = Convert.ToInt16(csv.GetField("HTAG")??"0"),
                    FullTimeResult = Convert.ToChar(csv.GetField("FTR")),
                    HalfTimeResult = Convert.ToChar(csv.GetField("HTR")), 
                    Attendance = Convert.ToInt32(csv.GetField("Attendance")??"0"),
                    Referee = Convert.ToString(csv.GetField("Referee")??"N/A")  
                };
                _matches.Add(match);
            }
        }
    }
    
    private static CsvConfiguration GetConfig()
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            MissingFieldFound = null,
       
        };
        return config;
    }
}