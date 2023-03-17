namespace ui.Models;

public static class Extensions
{
    public static IEnumerable<KeyValue> GetKvList(this Dictionary<string,string> kv)
    {
        return kv.Select(x => new KeyValue()
        {
            Key = x.Key,
            Value = x.Value
        });
    }

    public static Dictionary<string, string> GetDictionary(this List<KeyValue> list)
    {
        var dictionary = new Dictionary<string, string>();

        foreach (var item in list)
        {
            if (string.IsNullOrWhiteSpace(item.Key))
            {
                continue;
            }
            dictionary[item.Key] = item.Value;
        }

        return dictionary;
    }
}