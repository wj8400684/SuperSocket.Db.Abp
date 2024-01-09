using EntityFrameworkCore.UnitOfWork.Interfaces;
using SuperSocket.Channel;
using SuperSocket.ProtoBase;
using SuperSocket.Server;
using System.Net;

namespace SuperSocket.Db.Abp.Server;

public sealed partial class MyAppSession : AppSession
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
}
