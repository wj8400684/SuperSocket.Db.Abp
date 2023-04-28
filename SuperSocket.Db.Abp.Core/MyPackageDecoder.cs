using SuperSocket.ProtoBase;
using System.Buffers;

namespace SuperSocket.Db.Abp.Core;

/// <summary>
/// 包解码器
/// </summary>
public sealed class MyPackageDecoder : IPackageDecoder<MyPackage>
{
    private const int HeaderSize = sizeof(short);

    private readonly IPackageFactoryPool _packageFactoryPool;

    public MyPackageDecoder(IPackageFactoryPool packageFactoryPool)
    {
        _packageFactoryPool = packageFactoryPool;
    }

    public MyPackage Decode(ref ReadOnlySequence<byte> buffer, object context)
    {
        var reader = new SequenceReader<byte>(buffer);

        reader.Advance(HeaderSize);

        //读取 command
        reader.TryRead(out var command);

        var packetFactory = _packageFactoryPool.Get((MyCommand)command) ?? throw new ProtocolException($"命令：{command}未注册");

        var package = packetFactory.Create();

        package.DecodeBody(ref reader, package);

        return package;
    }
}
