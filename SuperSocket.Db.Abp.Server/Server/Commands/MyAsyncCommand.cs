using SuperSocket.Command;

namespace SuperSocket.Db.Abp.Server.Commands;

public abstract class MyAsyncCommand<TPackage> : IAsyncCommand<MyAppSession, MyPackage>
    where TPackage : MyPackage
{
    ValueTask IAsyncCommand<MyAppSession, MyPackage>.ExecuteAsync(MyAppSession session, MyPackage package) => SchedulerAsync(session, package, session.ConnectionToken);

    protected virtual async ValueTask SchedulerAsync(MyAppSession session, MyPackage package, CancellationToken cancellationToken)
    {
        var request = (TPackage)package;

        await ExecuteAsync(session, request, cancellationToken);
    }

    protected abstract ValueTask ExecuteAsync(MyAppSession session, TPackage package, CancellationToken cancellationToken);
}

public abstract class MyAsyncRespCommand<TPackage, TRespPackage> : IAsyncCommand<MyAppSession, MyPackage>
    where TPackage : MyPackage
    where TRespPackage : MyRespPackage, new()
{
    ValueTask IAsyncCommand<MyAppSession, MyPackage>.ExecuteAsync(MyAppSession session, MyPackage package) => SchedulerAsync(session, package, session.ConnectionToken);

    private readonly IPackageFactory _responseFactory;

    public MyAsyncRespCommand(IPackageFactoryPool packetFactoryPool)
    {
        _responseFactory = packetFactoryPool.Get<TRespPackage>();
    }

    protected TRespPackage CreateResponse(ulong identifier)
    {
        var resp = (TRespPackage)_responseFactory.Create();
        resp.Identifier = identifier;

        return resp;
    }

    protected virtual async ValueTask SchedulerAsync(MyAppSession session, MyPackage package, CancellationToken cancellationToken)
    {
        TRespPackage respPackage;
        var request = (TPackage)package;

        try
        {
            respPackage = await ExecuteAsync(session, request, cancellationToken);
        }
        catch (Exception e)
        {
            respPackage = CreateResponse(package.Identifier);
            respPackage.ErrorMessage = e.Message;
        }

        await session.SendPackageAsync(respPackage);
    }

    protected abstract ValueTask<TRespPackage> ExecuteAsync(MyAppSession session, TPackage package, CancellationToken cancellationToken);
}

