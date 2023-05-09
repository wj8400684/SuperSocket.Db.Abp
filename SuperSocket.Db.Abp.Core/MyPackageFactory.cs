using System.Collections.Concurrent;

namespace SuperSocket.Db.Abp.Core;

/// <summary>
/// 带有恢复的包创建工厂
/// </summary>
/// <typeparam name="TPackage"></typeparam>
public sealed class MyPackageFactory<TPackage> :
    IPackageFactory where TPackage :
    MyPackage, new()
{
    public MyPackageFactory()
    {
        PackageType = typeof(TPackage);
    }

    public Type PackageType { get; private set; }

    public MyPackage Create()
    {
        return new TPackage();
    }
}
