using api.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

// TODO: Look into using NLog instead of the built in stuff to get better audit logging
[ApiController]
[ApiExplorerSettings(IgnoreApi = true)] // Needed so swagger doesn't crash
public class ErrorController : ControllerBase
{
    private readonly ILogger<ErrorController> _logger;

    public ErrorController(ILogger<ErrorController> logger)
    {
        _logger = logger;
    }
    
    [Route("/error")] // This allows it to handle all VERBs
    public IActionResult HandleError()
    {
        var exceptionHandlerPathFeature =
            HttpContext.Features.Get<IExceptionHandlerPathFeature>();

        var e = exceptionHandlerPathFeature?.Error;

        if (e == null)
        {
            return StatusCode(500, "Error missing");
        }
        
        _logger.LogError(e, "Caught error");

        if (e is UnauthorizedAccessException error)
        {
            _logger.LogWarning("{Message}", e.Message);
            return StatusCode(403, new ErrorMessage(error.Message));
        }

        return StatusCode(500, new ErrorMessage(e.Message));
    }
}
