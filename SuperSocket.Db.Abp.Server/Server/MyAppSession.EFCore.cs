using EntityFrameworkCore.UnitOfWork.Interfaces;
using SuperSocket.Channel;
using SuperSocket.ProtoBase;
using SuperSocket.Server;
using System.Net;

namespace SuperSocket.Db.Abp.Server;

public sealed partial class MyAppSession
{
    internal async ValueTask ExecuteDbAsync(Func<IUnitOfWork, ValueTask> handler)
    {
        await using var scope = Server.ServiceProvider.CreateAsyncScope();
        using var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        await handler.Invoke(unitOfWork);
    }

    internal async ValueTask<TResult> ExecuteDbAsync<TResult>(Func<IUnitOfWork, ValueTask<TResult>> handler)
    {
        await using var scope = Server.ServiceProvider.CreateAsyncScope();
        using var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        return await handler.Invoke(unitOfWork);
    }
}
