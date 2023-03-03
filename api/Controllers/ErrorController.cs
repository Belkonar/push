using api.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[ApiExplorerSettings(IgnoreApi = true)] // Needed so swagger doesn't crash
public class ErrorController : ControllerBase
{
    [Route("/error")] // This allows it to handle all VERBs
    public IActionResult HandleError()
    {
        var exceptionHandlerPathFeature =
            HttpContext.Features.Get<IExceptionHandlerPathFeature>();

        if (exceptionHandlerPathFeature?.Error is UnauthorizedAccessException error)
        {
            return StatusCode(403, new ErrorMessage(error.Message));
        }
        
        Console.WriteLine(exceptionHandlerPathFeature?.Error?.Message);

        return StatusCode(500, new ErrorMessage("I got no clue"));
    }
}
