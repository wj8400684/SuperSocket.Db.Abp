using Microsoft.AspNetCore.Mvc;
using SuperSocket.Db.Abp.Core.Packages;

namespace SuperSocket.Db.Abp.Server.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class ClientController : ControllerBase
{
    private readonly ILogger<SessionController> _logger;
    private readonly IAsyncSessionContainer _sessionContainer;

    public ClientController(ILogger<SessionController> logger, IAsyncSessionContainer sessionContainer)
    {
        _logger = logger;
        _sessionContainer = sessionContainer;
    }

    [HttpGet("Restart")]
    public async ValueTask<IActionResult> RestartAsync(string id, CancellationToken cancellationToken)
    {
        if (await _sessionContainer.GetSessionByIDAsync(id) is not MyAppSession session)
            return NotFound("客户端不在线");

        try
        {
            await session.SendPackageAsync(new ClientRestartPackage());
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }

        return Ok();
    }
}