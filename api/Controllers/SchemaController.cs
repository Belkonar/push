using Json.More;
using Json.Schema;
using Json.Schema.Generation;
using Microsoft.AspNetCore.Mvc;
using shared.Models.Pipeline;
using shared.View;

namespace api.Controllers;

[ApiController]
[Route("[controller]")]
public class SchemaController : ControllerBase
{
    [HttpGet("PipelineVersion")]
    public ActionResult GetSchema()
    {
        var builder = new JsonSchemaBuilder();

        var schema = builder.FromType<PipelineVersion>().Build();

        Console.WriteLine();

        return new ContentResult()
        {
            Content = schema.ToJsonDocument().RootElement.ToJsonString(),
            ContentType = "application/json",
            StatusCode = 200
        };
    }
}