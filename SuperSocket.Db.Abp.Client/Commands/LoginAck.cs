
using SuperSocket.Client.Command;
using SuperSocket.Db.Abp.Core;

namespace SuperSocket.Db.Abp.Client;

[Command(Key = (byte)MyCommand.LoginAck)]
public sealed class LoginAck : MyAsyncCommandAsync<LoginRespPackage>
{
    protected override ValueTask ExecuteAsync(MyClient client, LoginRespPackage package)
    {
        return client.TryDispatchAsync(package);
    }
}
