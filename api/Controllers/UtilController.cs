using api.Logic;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using shared.View;

namespace api.Controllers;

[ApiController]
[Route("[controller]")]
public class UtilController : ControllerBase
{
    private readonly IMongoDatabase _database;
    private readonly PipelineLogic _pipelineLogic;

    public UtilController(IMongoDatabase database, PipelineLogic pipelineLogic)
    {
        _database = database;
        _pipelineLogic = pipelineLogic;
    }
    
    [HttpGet("fill")]
    public async Task<IActionResult> Fill()
    {
        const string policy = @"package main

global_admin { true }";
        
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

        return Ok("filled basic info");
    }
    
    /// <summary>
    /// Reset the global policy to one that works.
    /// TODO: Delete before live use
    /// </summary>
    /// <returns></returns>
    [HttpGet("reset")]
    public async Task<IActionResult> Reset()
    {
        throw new NotImplementedException();
    }

    [HttpGet()]
    public async Task<IActionResult> Test()
    {
        return Ok();
    }

    
}

// TODO: Move this lol
