
using SuperSocket.Client.Command;
using SuperSocket.Db.Abp.Core;

namespace SuperSocket.Db.Abp.Client;

public abstract class MyAsyncCommandAsync<TPacket> : IAsyncCommand<MyPackage> where TPacket : MyPackage
{
    protected abstract ValueTask ExecuteAsync(MyClient client, TPacket package);

    protected async virtual ValueTask OnSchedulerAsync(MyClient client, MyPackage package)
    {
        await ExecuteAsync(client, (TPacket)package);
    }

    ValueTask IAsyncCommand<MyPackage>.ExecuteAsync(object sender, MyPackage package) => OnSchedulerAsync((MyClient)sender, package);
}

public abstract class MyAsyncCommandAsync<TPacket, TRespPacket> : IAsyncCommand<MyPackage>
    where TPacket : MyPackage
    where TRespPacket : MyRespPackage, new()
{
    protected abstract ValueTask<TRespPacket> ExecuteAsync(MyClient client, TPacket package);

    protected async virtual ValueTask OnSchedulerAsync(MyClient client, MyPackage package)
    {
        TRespPacket resp;

        try
        {
            resp = await ExecuteAsync(client, (TPacket)package);
        }
        catch (Exception ex)
        {
            resp = new TRespPacket
            {
                ErrorMessage = ex.Message,
                Identifier = package.Identifier
            };//未知错误
        }

        await client.SendAsync(resp);
    }

    ValueTask IAsyncCommand<MyPackage>.ExecuteAsync(object sender, MyPackage package) => OnSchedulerAsync((MyClient)sender, package);
}