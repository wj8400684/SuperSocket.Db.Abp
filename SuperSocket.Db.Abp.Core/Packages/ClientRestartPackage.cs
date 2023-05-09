using MemoryPack;

namespace SuperSocket.Db.Abp.Core.Packages;

[MemoryPackable]
public sealed partial class ClientRestartPackage : MyPackage
{
    public ClientRestartPackage() : base(MyCommand.ClientRestart)
    {
    }
}