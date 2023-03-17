namespace shared.Interfaces;

public interface IGithub
{
    /// <summary>
    /// Download a zip folder of the given repo at the specific ref
    /// </summary>
    /// <param name="source">The source repo (in HTTP form)</param>
    /// <param name="reference">Any valid git ref for the repo</param>
    /// <returns>The zip in binary</returns>
    Task<byte[]> GetZip(string source, string reference);
    
    /// <summary>
    /// Get the actual SHA of a ref (for dealing with branches)
    /// </summary>
    /// <param name="source">The source repo (in HTTP form)</param>
    /// <param name="reference">Any valid git ref for the repo</param>
    /// <returns>The SHA1 of the ref</returns>
    Task<string> GetReference(string source, string reference);
}