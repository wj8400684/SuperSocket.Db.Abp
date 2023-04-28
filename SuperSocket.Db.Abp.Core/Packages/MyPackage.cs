using SuperSocket.Db.Abp.Core.Extensions;
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

public abstract class MyRespPackage : MyPackage
{
    public bool SuccessFul { get; set; }

    public MyErrorCode ErrorCode { get; set; }

    public string? ErrorMessage { get; set; }

    protected MyRespPackage(MyCommand key)
        : base(key)
    {
    }

    public override int Encode(IBufferWriter<byte> writer)
    {
        var length = writer.Write(SuccessFul ? (byte)1 : (byte)0);
        length += writer.Write((byte)ErrorCode);

        if (SuccessFul)
            return length;

        if (!string.IsNullOrWhiteSpace(ErrorMessage))
            length +=  writer.WriteEncoderString(ErrorMessage);

        return length;
    }

    protected internal override void DecodeBody(ref SequenceReader<byte> reader, object? context)
    {
        reader.TryRead(out var successFul);
        SuccessFul = successFul == 1;

        reader.TryRead(out var errorCode);
        ErrorCode = (MyErrorCode)errorCode;

        if (SuccessFul)
            return;

        reader.TryReadEncoderString(out var errorMessage);
        ErrorMessage = errorMessage;
    }

    public override void Dispose()
    {
        SuccessFul = default;
        ErrorCode = default;
        ErrorMessage = default;
    }
}
