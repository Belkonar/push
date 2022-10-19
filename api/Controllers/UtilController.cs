using data;
using data.ORM;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[Route("[controller]")]
public class UtilController : ControllerBase
{
    private readonly MainContext _context;

    public UtilController(MainContext context)
    {
        _context = context;
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
}