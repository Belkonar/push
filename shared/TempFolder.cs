namespace shared;

/// <summary>
/// Generates a temporary folder, disposing deletes it
/// </summary>
public class TempFolder : IDisposable
{
    public string Dir { get; }

    private readonly bool _shouldClear = true;
    
    public TempFolder()
    {
        Dir = Path.Join(Path.GetTempPath(), Guid.NewGuid().ToString());

        if (!Directory.Exists(Dir))
        {
            Directory.CreateDirectory(Dir);
        }
    }

    public TempFolder(string path, bool shouldClear = false)
    {
        _shouldClear = shouldClear;
        Dir = path;
    }

    public string GetFile(string fileName)
    {
        return Path.Join(Dir, fileName);
    }
    
    /// <summary>
    /// Get a file with a randomized name
    /// </summary>
    /// <returns>filepath</returns>
    public string GetFile()
    {
        return GetFile(Guid.NewGuid().ToString());
    }
    
    public void Dispose()
    {
        if (_shouldClear && Directory.Exists(Dir))
        {
            Directory.Delete(Dir, true);
        }
        
    }
}