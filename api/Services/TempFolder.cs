namespace api.Services;

/// <summary>
/// Generates a temporary folder, disposing deletes it
/// </summary>
public class TempFolder : IDisposable
{
    public string Dir { get; }
    
    public TempFolder()
    {
        Dir = Path.Join(Path.GetTempPath(), Guid.NewGuid().ToString());

        if (!Directory.Exists(Dir))
        {
            Directory.CreateDirectory(Dir);
        }
    }

    public string GetFile(string fileName)
    {
        return Path.Join(Dir, fileName);
    }
    
    public void Dispose()
    {
        if (Directory.Exists(Dir))
        {
            Directory.Delete(Dir, true);
        }
        
    }
}