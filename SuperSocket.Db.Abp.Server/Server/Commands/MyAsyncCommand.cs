using SuperSocket.Command;

namespace SuperSocket.Db.Abp.Server.Commands;

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

    protected TRespPackage CreateResponse() => (TRespPackage)_responseFactory.Create();

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
            respPackage = CreateResponse();
            respPackage.SuccessFul = false;
            respPackage.ErrorMessage = e.Message;
        }
        finally
        {
            request.Dispose();
        }

        try
        {
            await session.SendPackageAsync(respPackage);
        }
        finally
        {
            respPackage.Dispose();
        }
    }

    protected abstract ValueTask<TRespPackage> ExecuteAsync(MyAppSession session, TPackage package, CancellationToken cancellationToken);
}

