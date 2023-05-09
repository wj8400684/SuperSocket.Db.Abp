using MemoryPack;
using System.Buffers;

namespace SuperSocket.Db.Abp.Core;

[MemoryPackable]
public sealed partial class RegisterPackage : MyPackage
{
    public RegisterPackage() : base(MyCommand.Register)
    {
    }

    public string? Username { get; set; }

    public string? Password { get; set; }

    public string? Email { get; set; }

    /// <summary>
    /// 这里可以用一些常用序列化框架如 protobuf messagePack memoryPack 可以切换至 protobuf分支
    /// </summary>
    /// <param name="writer"></param>
    /// <returns></returns>
    public override int Encode(IBufferWriter<byte> writer)
    {
        ArgumentException.ThrowIfNullOrEmpty(Username);
        ArgumentException.ThrowIfNullOrEmpty(Password);
        ArgumentException.ThrowIfNullOrEmpty(Email);

        return base.Encode(writer);
    }
}

[MemoryPackable]
public sealed partial class RegisterRespPackage : MyRespPackage
{
    public RegisterRespPackage() : base(MyCommand.RegisterAck)
    {
    }
}
