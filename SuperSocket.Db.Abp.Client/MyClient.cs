using Microsoft.Extensions.Logging;
using SuperSocket.Client;
using SuperSocket.Client.Command;
using SuperSocket.Db.Abp.Core;
using SuperSocket.ProtoBase;
using System.Net;

namespace SuperSocket.Db.Abp.Client;

public sealed class MyClient : EasyCommandClient<MyCommand, MyPackage>
{
    private readonly IEasyClient<MyPackage, MyPackage> _easyClient;
    private readonly PacketDispatcher _packetDispatcher = new();
    private readonly PacketIdentifierProvider _packetIdentifierProvider = new();

    public MyClient(
        IPackageHandler<MyCommand, MyPackage> packageHandler,
        IPipelineFilter<MyPackage> pipelineFilter,
        IPackageEncoder<MyPackage> packageEncoder,
        ILogger<MyClient> logger) : base(packageHandler, pipelineFilter, packageEncoder, logger)
    {
        _easyClient = this;
    }

    internal new ILogger Logger => base.Logger;

    internal new ValueTask<bool> ConnectAsync(EndPoint remoteEndPoint, CancellationToken cancellationToken)
    {
        //if (remoteEndPoint is IPEndPoint endPoint) udp
        //{
        //    AsUdp(endPoint);
        //    return ValueTask.FromResult(true);
        //}

        return base.ConnectAsync(remoteEndPoint, cancellationToken);
    }

    internal ValueTask<LoginRespPackage> LoginAsync(LoginPackage package, CancellationToken cancellationToken = default)
    {
        return GetResponseAsync<LoginRespPackage>(package, cancellationToken);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="packageInfo"></param>
    /// <returns></returns>
    public ValueTask TryDispatchAsync(MyPackage packageInfo)
    {
        _packetDispatcher.TryDispatch(packageInfo);

        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// 获取响应封包
    /// </summary>
    /// <typeparam name="TRespPacket"></typeparam>
    /// <param name="package"></param>
    /// <exception cref="TimeoutException"></exception>
    /// <exception cref="TaskCanceledException"></exception>
    /// <exception cref="Exception"></exception>
    /// <returns></returns>
    public ValueTask<TRespPacket> GetResponseAsync<TRespPacket>(MyPackage package) where TRespPacket : MyRespPackage
    {
        return GetResponseAsync<TRespPacket>(package, CancellationToken.None);
    }

    /// <summary>
    /// 获取响应封包
    /// </summary>
    /// <typeparam name="TRespPacket"></typeparam>
    /// <param name="packet"></param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="TimeoutException"></exception>
    /// <exception cref="TaskCanceledException"></exception>
    /// <exception cref="Exception"></exception>
    /// <returns></returns>
    public async ValueTask<TRespPacket> GetResponseAsync<TRespPacket>(MyPackage packet, CancellationToken cancellationToken) where TRespPacket : MyRespPackage
    {
        if (CancellationTokenSource == null)
            throw new Exception("没有连接到服务器");

        if (CancellationTokenSource.IsCancellationRequested)
            throw new TaskCanceledException("已经与服务器断开连接");

        cancellationToken.ThrowIfCancellationRequested();

        packet.Identifier = _packetIdentifierProvider.GetNextPacketIdentifier();

        using var packetAwaitable = _packetDispatcher.AddAwaitable<TRespPacket>(packet.Identifier);
        using var cancel = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, CancellationTokenSource!.Token);

        try
        {
            await _easyClient.SendAsync(packet);
        }
        catch (Exception e)
        {
            packetAwaitable.Fail(e);
            throw new Exception("发送封包抛出一个异常", e);
        }

        try
        {
            return await packetAwaitable.WaitAsync(cancel.Token);
        }
        catch (Exception e)
        {
            if (e is TimeoutException)
                throw new TimeoutException($"等待封包调度超时命令：{packet.Key}", e);

            throw new Exception("等待封包调度抛出一个异常", e);
        }
    }
}
