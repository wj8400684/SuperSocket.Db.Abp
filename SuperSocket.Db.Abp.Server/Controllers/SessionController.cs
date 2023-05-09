using Microsoft.AspNetCore.Mvc;
using SuperSocket.Db.Abp.Server.Model;

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
    public async ValueTask<IActionResult> CountAsync()
    {
        var count = await _sessionContainer.GetSessionCountAsync();

        return Ok(count);
    }

    [HttpGet("All")]
    public async ValueTask<SessionAllModel> AllAsync()
    {
        var sessions = await _sessionContainer.GetSessionsAsync<MyAppSession>();

        return new SessionAllModel(sessions.Select(session => new SessionItem(session.SessionID, session.IsLogined)));
    }
}