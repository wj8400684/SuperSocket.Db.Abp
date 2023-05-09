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
    private readonly PacketDispatcher _packetDispatcher = new();
    private readonly PacketIdentifierProvider _packetIdentifierProvider = new();

    public MyAppSession(IPackageEncoder<MyPackage> encoder)
    {
        _encoder = encoder;
    }

    internal bool IsLogined { get; set; }

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
        _packetIdentifierProvider.Reset();
        _packetDispatcher.CancelAll();
        _packetDispatcher.Dispose();

        return ValueTask.CompletedTask;
    }

    internal ValueTask SendPackageAsync(MyPackage package)
    {
        return Channel.IsClosed ? ValueTask.CompletedTask : Channel.SendAsync(_encoder, package);
    }

    #region Dispatch package

    internal ValueTask<TResponsePacket> GetResponsePacketAsync<TResponsePacket>(
         MyPackage package,
         TimeSpan responseTimeout,
         CancellationToken cancellationToken) where TResponsePacket : MyRespPackage
    {
        using var timeOut = new CancellationTokenSource(responseTimeout);
        return GetResponsePacketAsync<TResponsePacket>(package, ConnectionToken, cancellationToken, timeOut.Token);
    }

    internal ValueTask<TResponsePacket> GetResponsePacketAsync<TResponsePacket>(
         MyPackage package,
         CancellationToken cancellationToken) where TResponsePacket : MyRespPackage
    {
        return GetResponsePacketAsync<TResponsePacket>(package, ConnectionToken, cancellationToken);
    }

    internal async ValueTask<TResponsePacket> GetResponsePacketAsync<TResponsePacket>(
        MyPackage package,
        params CancellationToken[] tokens) where TResponsePacket : MyRespPackage
    {
        using var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(tokens);

        package.Identifier = _packetIdentifierProvider.GetNextPacketIdentifier();

        using var packetAwaitable = _packetDispatcher.AddAwaitable<TResponsePacket>(package.Identifier);

        this.LogDebug($"[{RemoteAddress}]: commandKey= {package.Key};Identifier= {package.Identifier} WaitAsync");

        try
        {
            await SendPackageAsync(package);
        }
        catch (Exception e)
        {
            packetAwaitable.Fail(e);
            this.LogError(e, $"[{RemoteAddress}]: commandKey= {package.Key};Identifier= {package.Identifier} WaitAsync 发送封包抛出一个异常");
        }

        try
        {
            //等待封包结果
            return await packetAwaitable.WaitAsync(tokenSource.Token);
        }
        catch (Exception e)
        {
            if (e is TimeoutException)
                this.LogError($"[{RemoteAddress}]: commandKey= {package.Key};Identifier= {package.Identifier} WaitAsync Timeout");

            throw;
        }
    }

    internal ValueTask TryDispatchAsync(MyPackage package)
    {
        var result = _packetDispatcher.TryDispatch(package);

        this.LogDebug($"[{RemoteAddress}]: commandKey= {package.Key};Identifier= {package.Identifier} TryDispatch result= {result}");

        return ValueTask.CompletedTask;
    }

    #endregion

    #region db

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

    #endregion
}
