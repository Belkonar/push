using api.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using shared.Models.Pipeline;

namespace api.Controllers;

[ApiController]
[Route("[controller]")]
public class UtilController : ControllerBase
{
    private readonly IMongoDatabase _database;
    private readonly ConfigService _configService;

    public UtilController(IMongoDatabase database, ConfigService configService)
    {
        _database = database;
        _configService = configService;
    }
    
    [HttpGet("fill")]
    public async Task<string> Fill()
    {
        var pipelineId = Guid.Parse("bcb885d2-b0dd-4cad-85a8-14de0b67d012");

        var pipelineCollection = _database.GetCollection<Pipeline>("pipelines");

        var filter = Builders<Pipeline>.Filter
            .Eq(x => x.Id, pipelineId);

        var pipeline = await pipelineCollection.Find(filter).FirstOrDefaultAsync();

        if (pipeline == null)
        {
            pipeline = new Pipeline()
            {
                Id = pipelineId,
                Description = "basic pipeline for testing purposes",
                Name = "nestjs"
            };

            await pipelineCollection.InsertOneAsync(pipeline);
        }

        /*
         * var nestjsPipeline = await _context.Pipelines.FindAsync(Guid.Parse("bcb885d2-b0dd-4cad-85a8-14de0b67d012"));
        if (nestjsPipeline == null)
        {
            await _context.AddAsync(new PipelineDTO()
            {
                Id = Guid.Parse("bcb885d2-b0dd-4cad-85a8-14de0b67d012"),
                Description = "basic pipeline for testing purposes",
                Name = "nestjs"
            });
        }
         */

        return "filled basic info";
    }

    [HttpGet()]
    public async Task<string> Test()
    {
        await _configService.Set("babbers", new Dictionary<string, string>()
        {
            { "hello", "world" }
        });
        
        return "";
    }

    [HttpGet("read")]
    public async Task<Dictionary<string, string>> Reader()
    {
        return await _configService.Get<Dictionary<string, string>>("babbers");
    }
}

// TODO: Move this lol
