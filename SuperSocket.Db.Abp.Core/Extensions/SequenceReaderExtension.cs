using SuperSocket.ProtoBase;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace SuperSocket.Db.Abp.Core.Extensions;

public static class SequenceReaderExtension
{
    public static bool TryReadEncoderString(ref this SequenceReader<byte> reader, [MaybeNullWhen(false)] out string value)
    {
        value = null;

        var result = reader.TryReadBigEndian(out ushort valueLength);

        if (!result || valueLength == 0) 
            return false;

        value = reader.ReadString(Encoding.UTF8, valueLength);

        return true;
    }
}
