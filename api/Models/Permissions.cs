namespace api.Models;

public class Permissions : List<string>
{
    public bool HasPermission(string key)
    {
        return Contains("global_admin") || Contains(key);
    }

    public bool IsMissing(string key)
    {
        return !HasPermission(key);
    }

    public static string GlobalPolicyManage = "global_policy_manage";

    public Permissions() {}
    
    public Permissions(IEnumerable<string> list) : base(list) {}
}