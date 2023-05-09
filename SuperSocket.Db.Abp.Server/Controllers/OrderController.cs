using Microsoft.AspNetCore.Mvc;
using SuperSocket.Db.Abp.Core.Packages;

namespace SuperSocket.Db.Abp.Server.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class OrderController : ControllerBase
{
    private readonly ILogger<SessionController> _logger;
    private readonly IAsyncSessionContainer _sessionContainer;

    public OrderController(ILogger<SessionController> logger, IAsyncSessionContainer sessionContainer)
    {
        _logger = logger;
        _sessionContainer = sessionContainer;
    }

    [HttpGet("Add")]
    public async ValueTask<IActionResult> AddAsync(string id, CancellationToken cancellationToken)
    {
        if (await _sessionContainer.GetSessionByIDAsync(id) is not MyAppSession session)
            return NotFound("客户端不在线");

        OrderAddRespPackage resp;

        var package = new OrderAddPackage
        {
            Id = id,
        };

        try
        {
            resp = await session.GetResponsePacketAsync<OrderAddRespPackage>(package, cancellationToken);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }

        return Ok(resp);
    }
}