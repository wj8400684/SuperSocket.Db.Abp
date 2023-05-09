using SuperSocket.Db.Abp.Core.Packages;
using SuperSocket.Db.Abp.Server.Commands;

namespace SuperSocket.Db.Abp.Server.Server.Commands.Ack;

/// <summary>
/// 添加订单回复
/// </summary>
[MyLoginCommandFilter]
[MyCommand(MyCommand.OrderAddAck)]
public class OrderAddAckCommand : MyAsyncCommand<OrderAddRespPackage>
{
    protected override ValueTask ExecuteAsync(MyAppSession session, OrderAddRespPackage package, CancellationToken cancellationToken)
    {
        return session.TryDispatchAsync(package);
    }
}
