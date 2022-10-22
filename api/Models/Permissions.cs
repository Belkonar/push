namespace api.Models;

public class Permissions : List<string>
{
    public bool HasPermission(string key)
    {
        return Contains("global_admin") || Contains(key);
    }

    public static string GlobalPolicyManage = "global_policy_manage";
}