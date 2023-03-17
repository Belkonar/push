
namespace shared.Interfaces;

public interface IUtilController
{
    public Task<string> Fill();
    public Task<string> Test();
    public Task<Dictionary<string,string>> Reader();
}
