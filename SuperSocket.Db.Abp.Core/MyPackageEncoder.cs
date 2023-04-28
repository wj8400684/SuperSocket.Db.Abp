using System.Buffers;
using SuperSocket.ProtoBase;
using System.Buffers.Binary;
using SuperSocket.Db.Abp.Core.Extensions;

namespace SuperSocket.Db.Abp.Core;

/// <summary>
/// 包编码器
/// </summary>
public sealed class MyPackageEncoder : IPackageEncoder<MyPackage>
{
    private const byte HeaderSize = sizeof(short);

    public int Encode(IBufferWriter<byte> writer, MyPackage pack)
    {
        #region 获取头部字节缓冲区

        var headSpan = writer.GetSpan(HeaderSize);
        writer.Advance(HeaderSize);

        #endregion

        #region 写入 command

        var length = writer.Write((byte)pack.Key);

        #endregion

        #region 写入内容

        length += pack.Encode(writer);

        #endregion

        #region 写入 body 的长度

        BinaryPrimitives.WriteInt16LittleEndian(headSpan, (short)length);

        #endregion

        return HeaderSize + length;
    }
}