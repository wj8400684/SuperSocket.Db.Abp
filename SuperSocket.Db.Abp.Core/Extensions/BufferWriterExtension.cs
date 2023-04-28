using System.Buffers;
using System.Buffers.Binary;
using System.Text;
using SuperSocket.ProtoBase;

namespace SuperSocket.Db.Abp.Core.Extensions;

public static class BufferWriterExtension
{
    public static int Write(this IBufferWriter<byte> writer, byte value)
    {
        const int bufferSize = sizeof(byte);

        var span = writer.GetSpan(bufferSize);
        writer.Advance(bufferSize);
        span[0] = value;

        return bufferSize;
    }

    public static int WriteBigEndian(this IBufferWriter<byte> writer, ushort value)
    {
        const int bufferSize = sizeof(ushort);

        var span = writer.GetSpan(bufferSize);
        writer.Advance(bufferSize);
        BinaryPrimitives.WriteUInt16BigEndian(span, value);

        return bufferSize;
    }

    public static int WriteEncoderString(this IBufferWriter<byte> writer, string value)
    {
        const int bufferSize = sizeof(ushort);

        var span = writer.GetSpan(bufferSize);
        writer.Advance(bufferSize);

        var length = writer.Write(value, Encoding.UTF8);

        BinaryPrimitives.WriteUInt16BigEndian(span, (ushort)length);

        return bufferSize + length;
    }
}
