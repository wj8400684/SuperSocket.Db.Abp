﻿using SuperSocket.Db.Abp.Core.Extensions;
using System.Buffers;

namespace SuperSocket.Db.Abp.Core;

public sealed class LoginPackage : MyPackage
{
    public LoginPackage() : base(MyCommand.Login)
    {
    }

    public string? Username { get; set; }

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

        var length = writer.WriteEncoderString(Username);
        length += writer.WriteEncoderString(Password);

        return length;
    }

    /// <summary>
    /// 这里可以用一些常用序列化框架如 protobuf messagePack memoryPack 可以切换至 protobuf分支
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="context"></param>
    protected internal override void DecodeBody(ref SequenceReader<byte> reader, object? context)
    {
        if (reader.TryReadEncoderString(out var username))
            Username = username;

        if (reader.TryReadEncoderString(out var password))
            Password = password;
    }

    public override void Dispose()
    {
        Username = default; 
        Password = default;
        base.Dispose();
    }
}

public sealed class LoginRespPackage : MyRespPackage
{
    public LoginRespPackage() : base(MyCommand.LoginAck)
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
