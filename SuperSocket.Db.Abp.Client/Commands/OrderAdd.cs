using Microsoft.Extensions.Logging;
using SuperSocket.Client.Command;
using SuperSocket.Db.Abp.Core;
using SuperSocket.Db.Abp.Core.Packages;

namespace SuperSocket.Db.Abp.Client.Commands;

[Command(Key = (byte)MyCommand.OrderAdd)]
public sealed class OrderAdd : MyAsyncCommandAsync<OrderAddPackage, OrderAddRespPackage>
{
    protected override ValueTask<OrderAddRespPackage> ExecuteAsync(MyClient client, OrderAddPackage package)
    {
        client.Logger.LogInformation($"添加订单 {package.Id}");

        return ValueTask.FromResult(new OrderAddRespPackage
        {
            Identifier = package.Identifier,
            SuccessFul = true,
        });
    }
}
