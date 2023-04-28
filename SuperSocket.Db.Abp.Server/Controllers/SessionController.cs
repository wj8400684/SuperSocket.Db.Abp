using Microsoft.AspNetCore.Mvc;

namespace SuperSocket.Db.Abp.Server.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class SessionController : ControllerBase
{
    private readonly ILogger<SessionController> _logger;
    private readonly IAsyncSessionContainer _sessionContainer;

    public SessionController(ILogger<SessionController> logger, IAsyncSessionContainer sessionContainer)
    {
        _logger = logger;
        _sessionContainer = sessionContainer;
    }

    [HttpGet("Count")]
    public async ValueTask<IActionResult> CountSessionAsync()
    {
        var count = await _sessionContainer.GetSessionCountAsync();

        return Ok(count);
    }
}