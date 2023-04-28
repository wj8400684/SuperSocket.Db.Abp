using ProtoBuf;
using ProtoBuf.Meta;
using System.Buffers;

namespace SuperSocket.Db.Abp.Core;

[ProtoContract]
public sealed class LoginPackage : MyPackage
{
    public LoginPackage() : base(MyCommand.Login)
    {
    }

    [ProtoMember(1)]
    public string? Username { get; set; }

    [ProtoMember(2)]
    public string? Password { get; set; }

    /// <summary>
    /// 这里可以用一些常用序列化框架如 protobuf messagePack memoryPack 可以切换至 protobuf分支
    /// </summary>
    /// <param name="writer"></param>
    /// <returns></returns>
    public override int Encode(IBufferWriter<byte> writer)
    {
        ArgumentException.ThrowIfNullOrEmpty(Username);
        ArgumentException.ThrowIfNullOrEmpty(Password);

        using var stream = new MemoryStream();
        Serializer.Serialize(stream, this);
        var buffer = stream.ToArray();
        writer.Write(buffer);

        return buffer.Length;
    }

    /// <summary>
    /// 这里可以用一些常用序列化框架如 protobuf messagePack memoryPack 可以切换至 protobuf分支
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="context"></param>
    protected internal override void DecodeBody(ref SequenceReader<byte> reader, object? context)
    {
        Serializer.Deserialize(reader.UnreadSequence, this);
    }

    public override void Dispose()
    {
        Username = default;
        Password = default;
        base.Dispose();
    }
}

[ProtoContract]
public sealed class LoginRespPackage : MyRespPackage
{
    [ProtoMember(1)]
    public override bool SuccessFul { get; set; }

    [ProtoMember(2)]
    public override MyErrorCode ErrorCode { get; set; }

    [ProtoMember(3)]
    public override string? ErrorMessage { get; set; }

    public LoginRespPackage() : base(MyCommand.LoginAck)
    {
    }

    public override int Encode(IBufferWriter<byte> writer)
    {
        using var stream = new MemoryStream();
        Serializer.Serialize(stream, this);
        var buffer = stream.ToArray();
        writer.Write(buffer);

        return buffer.Length;
    }

    protected internal override void DecodeBody(ref SequenceReader<byte> reader, object? context)
    {
        Serializer.Deserialize(reader.UnreadSequence, this);
    }

    public override void Dispose()
    {
        SuccessFul = default; 
        ErrorCode = default; 
        ErrorMessage = default;
    }
}
