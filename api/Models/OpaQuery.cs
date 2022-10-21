using System.Text.Json.Serialization;

namespace api.Models;

/// <summary>
/// Base class for sending OPA rego queries.
/// </summary>
/// <typeparam name="T">Type of input data</typeparam>
public class OpaQuery<T>
{
    [JsonPropertyName("query")]
    public string Query { get; set; }
    
    [JsonPropertyName("input")]
    public T Input { get; set; }

    public OpaQuery(string query, T input)
    {
        Query = query;
        Input = input;
    }
}