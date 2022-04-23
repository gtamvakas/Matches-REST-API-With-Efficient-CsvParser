using System.Collections.Concurrent;
using cw2backend.Helpers;
using cw2backend.Models;

namespace cw2backend.Services;

public class ReaderService : IReaderService
{
    private readonly IMatchService _matchService;


    public ReaderService(IMatchService matchService)
    {
        _matchService = matchService;
    }

    public bool ParseFiles()
    {
        var matches = new BlockingCollection<Match>();
        var csvFiles = FileHelper.GetAllFiles("*.csv");
        Parallel.ForEach(csvFiles, csv =>
        {
            var csvHelperWrapper = new CsvHelperWrapper(csv, matches);
            csvHelperWrapper.ReadCsv();
        });

        return _matchService.WriteFromCsvToDb(matches);
    }
}