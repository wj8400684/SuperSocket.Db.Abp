using ProtoBuf;
using System.Buffers;

namespace SuperSocket.Db.Abp.Core;

[ProtoContract]
public sealed class RegisterPackage : MyPackage
{
    public RegisterPackage() : base(MyCommand.Register)
    {
    }

    [ProtoMember(1)]
    public string? Username { get; set; }

    [ProtoMember(2)]
    public string? Password { get; set; }

    [ProtoMember(3)]
    public string? Email { get; set; }

    public override int Encode(IBufferWriter<byte> writer)
    {
        ArgumentException.ThrowIfNullOrEmpty(Username);
        ArgumentException.ThrowIfNullOrEmpty(Password);
        ArgumentException.ThrowIfNullOrEmpty(Email);

        Serializer.Serialize(writer, this);

        return default;//返回值没啥用处
    }

    protected internal override void DecodeBody(ref SequenceReader<byte> reader, object? context)
    {
        Serializer.Deserialize(reader.UnreadSequence, this);
    }

    public override void Dispose()
    {
        Username = default;
        Password = default;
        Email = default;
    }
}

[ProtoContract]
public sealed class RegisterRespPackage : MyPackage
{
    public RegisterRespPackage() : base(MyCommand.RegisterAck)
    {
    }

    [ProtoMember(1)]
    public bool SuccessFul { get; set; }

    [ProtoMember(2)]
    public MyErrorCode ErrorCode { get; set; }

    [ProtoMember(3)]
    public string? ErrorMessage { get; set; }

    public override int Encode(IBufferWriter<byte> writer)
    {
        Serializer.Serialize(writer, this);

        return default;//返回值没啥用处
    }

    protected internal override void DecodeBody(ref SequenceReader<byte> reader, object? context)
    {
        Serializer.Deserialize(reader.UnreadSequence, this);
    }
}
