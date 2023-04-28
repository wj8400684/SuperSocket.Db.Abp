namespace SuperSocket.Db.Abp.Core;

public interface IPackageFactoryPool
{
    IPackageFactory Get(MyCommand command);

    IPackageFactory Get<TPackage>();
}
