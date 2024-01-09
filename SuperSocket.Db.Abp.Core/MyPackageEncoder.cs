using System.Buffers;
using SuperSocket.ProtoBase;
using System.Buffers.Binary;
using SuperSocket.Db.Abp.Core.Extensions;

namespace SuperSocket.Db.Abp.Core;

/// <summary>
/// ????????
/// </summary>
public sealed class MyPackageEncoder : IPackageEncoder<MyPackage>
{
    private const byte HeaderSize = sizeof(short);

    public int Encode(IBufferWriter<byte> writer, MyPackage pack)
    {
        #region ??????????????

        var headSpan = writer.GetSpan(HeaderSize);
        writer.Advance(HeaderSize);

        #endregion

        #region д?? command

        var length = writer.Write((byte)pack.Key);

        #endregion

        #region д??????

        length += pack.Encode(writer);

        #endregion

        #region д?? body ?????

        BinaryPrimitives.WriteInt16LittleEndian(headSpan, (short)length);

        #endregion

        return HeaderSize + length;
    }
}