using EntityFrameworkCore.UnitOfWork.Interfaces;
using SuperSocket.Channel;
using SuperSocket.ProtoBase;
using SuperSocket.Server;
using System.Net;

namespace SuperSocket.Db.Abp.Server;

public sealed class MyAppSession : AppSession
{
    private CancellationTokenSource? _tokenSource;
    private readonly IPackageEncoder<MyPackage> _encoder;

    public MyAppSession(IPackageEncoder<MyPackage> encoder)
    {
        _encoder = encoder;
    }

    internal string RemoteAddress { get; private set; } = default!;

    internal CancellationToken ConnectionToken { get; private set; }

    protected override ValueTask OnSessionConnectedAsync()
    {
        _tokenSource = new CancellationTokenSource();
        RemoteAddress = ((IPEndPoint)RemoteEndPoint).Address.ToString();

        return ValueTask.CompletedTask;
    }

    protected override ValueTask OnSessionClosedAsync(CloseEventArgs e)
    {
        RemoteAddress = string.Empty;
        _tokenSource?.Cancel();
        _tokenSource?.Dispose();

        return ValueTask.CompletedTask;
    }

    internal ValueTask SendPackageAsync(MyPackage package)
    {
        return Channel.IsClosed ? ValueTask.CompletedTask : Channel.SendAsync(_encoder, package);
    }

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
