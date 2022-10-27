using System.Text.Json;
using System.Text.Json.Serialization;
using api.Services;
using data;
using data.ORM;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[Route("[controller]")]
public class UtilController : ControllerBase
{
    private readonly MainContext _context;
    private readonly OpaService _opaService;

    public UtilController(MainContext context, OpaService opaService)
    {
        _context = context;
        _opaService = opaService;
    }
    
    [HttpGet("fill")]
    public async Task<IActionResult> Fill()
    {
        var global = await _context.Policies.FindAsync("global");

        if (global == null)
        {
            await _context.AddAsync(new PolicyDto()
            {
                Key = "global",
                Policy = ""
            });
        }

        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpGet()]
    public async Task<IActionResult> Test()
    {
        return Ok();
    }

    
}

// TODO: Move this lol