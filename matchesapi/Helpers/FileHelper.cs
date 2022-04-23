namespace cw2backend.Helpers;

public class FileHelper
{

    public static IEnumerable<string> GetAllFiles(string? pattern = null)
    {
        return string.IsNullOrEmpty(pattern) ? 
            Directory.EnumerateFiles(Environment.CurrentDirectory) : 
            Directory.EnumerateFiles(Environment.CurrentDirectory, pattern, SearchOption.AllDirectories);
    }
}  