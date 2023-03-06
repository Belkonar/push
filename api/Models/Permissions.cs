namespace api.Models;

public class Permissions : List<string>
{
    [Obsolete("use Permissions.Check instead")]
    public bool HasPermission(string key)
    {
        return Contains("global_admin") || Contains(key);
    }

    [Obsolete("use Permissions.Check instead")]
    public bool IsMissing(string key)
    {
        return !HasPermission(key);
    }

    /// <summary>
    /// Check the user against a list of permissions and throw an exception
    /// if they do not have the specific permission
    /// </summary>
    /// <param name="perms">a list of permissions to check against</param>
    /// <param name="errMessage">the message to add to the exception if that occurs</param>
    /// <exception cref="UnauthorizedAccessException"></exception>
    public void Check(IEnumerable<string> perms, string errMessage = "unauthorized")
    {
        if (Contains("global_admin") || perms.Any(Contains))
        {
            return;
        }
        
        throw new UnauthorizedAccessException(errMessage);
    }
    
    /// <summary>
    /// Check the user against a permission and throw an exception
    /// if they do not have the specific permission
    /// </summary>
    /// <param name="perm">the permission to check against</param>
    /// <param name="errMessage">the message to add to the exception if that occurs</param>
    /// <exception cref="UnauthorizedAccessException"></exception>
    public void Check(string perm, string errMessage = "unauthorized")
    {
        Check(new []{ perm }, errMessage);
    }
    
    /// <summary>
    /// Just check for admin permission
    /// </summary>
    /// <param name="errMessage">the message to add to the exception if that occurs</param>
    public void Check(string errMessage = "unauthorized")
    {
        Check(new List<string>(), errMessage);
    }

    public static string GlobalPolicyManage = "global_policy_manage";

    public Permissions() {}
    
    public Permissions(IEnumerable<string> list) : base(list) {}
}