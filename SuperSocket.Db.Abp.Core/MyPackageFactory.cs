using System.Collections.Concurrent;

namespace SuperSocket.Db.Abp.Core;

public sealed class MyPackageFactory<TPackage> :
    IPackageFactory where TPackage :
    MyPackage, new()
{
    private const int DefaultMaxCount = 10;
    private readonly ConcurrentQueue<MyPackage> _packagePool = new();

    public MyPackageFactory()
    {
        for (var i = 0; i < DefaultMaxCount; i++)
        {
            var packet = new TPackage();

            packet.Initialization(this);

            _packagePool.Enqueue(packet);
        }

        PackageType = typeof(TPackage);
    }

    public Type PackageType { get; private set; }

    public MyPackage Create()
    {
        if (_packagePool.TryDequeue(out var package))
            return package;

        var packet = new TPackage();

        packet.Initialization(this);

        return packet;
    }

    public void Return(MyPackage package)
    {
        _packagePool.Enqueue(package);
    }
}
