using SuperSocket.Db.Abp.Core.Extensions;
using System.Buffers;

namespace SuperSocket.Db.Abp.Core;

public sealed class RegisterPackage : MyPackage
{
    public RegisterPackage() : base(MyCommand.Register)
    {
    }

    public string? Username { get; set; }

    public string? Password { get; set; }

    public string? Email { get; set; }

    public override int Encode(IBufferWriter<byte> writer)
    {
        ArgumentException.ThrowIfNullOrEmpty(Username);
        ArgumentException.ThrowIfNullOrEmpty(Password);
        ArgumentException.ThrowIfNullOrEmpty(Email);

        var length = writer.WriteEncoderString(Username);
        length += writer.WriteEncoderString(Password);
        length += writer.WriteEncoderString(Email);

        return length;
    }

    protected internal override void DecodeBody(ref SequenceReader<byte> reader, object? context)
    {
        if (reader.TryReadEncoderString(out var username))
            Username = username;

        if (reader.TryReadEncoderString(out var password))
            Password = password;

        if (reader.TryReadEncoderString(out var email))
            Email = email;
    }

    public override void Dispose()
    {
        Username = default;
        Password = default;
        Email = default;
        base.Dispose();
    }
}

public sealed class RegisterRespPackage : MyRespPackage
{
    public RegisterRespPackage() : base(MyCommand.RegisterAck)
    {
    }

    public override int Encode(IBufferWriter<byte> bufWriter)
    {
        return base.Encode(bufWriter);
    }

    protected internal override void DecodeBody(ref SequenceReader<byte> reader, object? context)
    {
        base.DecodeBody(ref reader, context);
    }

    public override void Dispose()
    {
        base.Dispose();
    }
}
