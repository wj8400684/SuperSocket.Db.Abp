using MemoryPack;
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

    [MemoryPackIgnore]
    public MyCommand Key { get; set; }

    public virtual void Initialization(IPackageFactory factory)
    {
        _packetFactory = factory;
    }

    public virtual int Encode(IBufferWriter<byte> bufWriter)
    {
        using var state = MemoryPackWriterOptionalStatePool.Rent(MemoryPackSerializerOptions.Utf8);
        var writer = new MemoryPackWriter<IBufferWriter<byte>>(ref bufWriter, state);
        writer.WriteValue(Type, this);
        var writtenCount = writer.WrittenCount;
        writer.Flush();

        return writtenCount;
    }

    protected internal virtual void DecodeBody(ref SequenceReader<byte> reader, object? context)
    {
        MemoryPackSerializer.Deserialize(Type, reader.UnreadSequence, ref context);
    }

    public override string ToString()
    {
        return System.Text.Json.JsonSerializer.Serialize(this, Type);
    }

    public virtual void Dispose()
    {
        _packetFactory?.Return(this);
    }
}

public abstract class MyRespPackage : MyPackage
{
    public bool SuccessFul { get; set; }

    public MyErrorCode ErrorCode { get; set; }

    public string? ErrorMessage { get; set; }

    protected MyRespPackage(MyCommand key)
        : base(key)
    {
    }

    public override void Dispose()
    {
        SuccessFul = default;
        ErrorCode = default;
        ErrorMessage = default;
    }
}
