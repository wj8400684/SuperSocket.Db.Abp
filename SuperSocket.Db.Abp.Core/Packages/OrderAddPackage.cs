using MemoryPack;
using System.Buffers;

namespace SuperSocket.Db.Abp.Core.Packages;

[MemoryPackable]
public sealed partial class OrderAddPackage : MyPackage
{
    public OrderAddPackage() : base(MyCommand.OrderAdd)
    {
    }

    public string? Id { get; set; }


    public override int Encode(IBufferWriter<byte> writer)
    {
        ArgumentException.ThrowIfNullOrEmpty(Id);

        return base.Encode(writer);
    }

    public override void Dispose()
    {
        Id = default;
        base.Dispose();
    }
}

[MemoryPackable]
public sealed partial class OrderAddRespPackage : MyRespPackage
{
    public OrderAddRespPackage() : base(MyCommand.OrderAddAck)
    {
    }
}