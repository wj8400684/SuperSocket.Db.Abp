using SuperSocket.ProtoBase;
using System.Buffers;

namespace SuperSocket.Db.Abp.Core;

public sealed class MyPipeLineFilter : FixedHeaderPipelineFilter<MyPackage>
{
    private const int HeaderSize = sizeof(short);

    public MyPipeLineFilter()
        : base(HeaderSize)
    {
    }

    public override MyPackage Filter(ref SequenceReader<byte> reader)
    {
        Decoder ??= new MyPackageDecoder(new MyPackageFactoryPool());

        return base.Filter(ref reader);
    }

    protected override int GetBodyLengthFromHeader(ref ReadOnlySequence<byte> buffer)
    {
        var reader = new SequenceReader<byte>(buffer);

        reader.TryReadLittleEndian(out short bodyLength);

        return bodyLength;
    }
}
