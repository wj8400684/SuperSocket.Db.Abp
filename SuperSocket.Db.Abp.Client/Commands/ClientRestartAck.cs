
using Microsoft.Extensions.Logging;
using SuperSocket.Client.Command;
using SuperSocket.Db.Abp.Core;
using SuperSocket.Db.Abp.Core.Packages;

namespace SuperSocket.Db.Abp.Client;

[Command(Key = (byte)MyCommand.ClientRestart)]
public sealed class ClientRestartAck : MyAsyncCommandAsync<ClientRestartPackage>
{
    protected override ValueTask ExecuteAsync(MyClient client, ClientRestartPackage package)
    {
        client.Logger.LogInformation($"重启当前客户端");

        return ValueTask.CompletedTask;
    }
}
