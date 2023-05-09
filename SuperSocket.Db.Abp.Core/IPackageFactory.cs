namespace SuperSocket.Db.Abp.Core;

public interface IPackageFactory
{
    Type PackageType { get; }

    MyPackage Create();
}
