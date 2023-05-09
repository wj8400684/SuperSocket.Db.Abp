using MemoryPack;
using System.Buffers;

namespace SuperSocket.Db.Abp.Core;

[MemoryPackable]
public sealed partial class LoginPackage : MyPackage
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

        return base.Encode(writer);
    }
}

[MemoryPackable]
public sealed partial class LoginRespPackage : MyRespPackage
{
    public LoginRespPackage() : base(MyCommand.LoginAck)
    {
    }
}
