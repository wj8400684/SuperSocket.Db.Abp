using SuperSocket.ProtoBase;
using System.Buffers;

namespace SuperSocket.Db.Abp.Core;

public abstract class MyPackage : IKeyedPackageInfo<MyCommand>, IDisposable
{
    private IPackageFactory? _packetFactory;

    protected readonly Type Type;

    protected MyPackage(MyCommand key)
    {
        Key = key;
        Type = GetType();
    }

    public MyCommand Key { get; set; }

    public virtual void Initialization(IPackageFactory factory)
    {
        _packetFactory = factory;
    }

    public abstract int Encode(IBufferWriter<byte> writer);

    protected internal abstract void DecodeBody(ref SequenceReader<byte> reader, object? context);

    public override string ToString()
    {
        return System.Text.Json.JsonSerializer.Serialize(this, Type);
    }

    public virtual void Dispose()
    {
        _packetFactory?.Return(this);
    }
}
