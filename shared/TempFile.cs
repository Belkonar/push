namespace shared;

public class TempFile : IDisposable
{
    public string FilePath { get; }

    public TempFile(string extension = "")
    {
        FilePath = Path.Join(Path.GetTempPath(), $"{Guid.NewGuid().ToString()}{extension}");
    }

    public string GetFileName()
    {
        return Path.GetFileName(FilePath);
    }
    
    public void Dispose()
    {
        if (File.Exists(FilePath))
        {
            File.Delete(FilePath);
        }
    }
}