using Json.More;
using Json.Schema;
using Json.Schema.Generation;
using Microsoft.AspNetCore.Mvc;
using shared.Models.Pipeline;

namespace api.Controllers;

[ApiController]
[Route("[controller]")]
public class SchemaController : ControllerBase
{
    // [HttpGet("PipelineVersion")]
    // public string GetSchema()
    // {
    //     var builder = new JsonSchemaBuilder();
    //
    //     var schema = builder.FromType<PipelineVersionCode>().Build();
    //
    //     Console.WriteLine();
    //
    //     return new ContentResult()
    //     {
    //         Content = schema.ToJsonDocument().RootElement.ToJsonString(),
    //         ContentType = "application/json",
    //         StatusCode = 200
    //     };
    // }
}