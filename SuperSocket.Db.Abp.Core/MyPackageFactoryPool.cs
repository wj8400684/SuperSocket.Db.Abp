using System.Data;

namespace SuperSocket.Db.Abp.Core;

/// <summary>
/// 包工厂池
/// </summary>
public sealed class MyPackageFactoryPool : IPackageFactoryPool
{
    private static readonly Dictionary<Type, MyCommand> CommandTypes = new();

    private readonly IPackageFactory[] _packetFactories;

    #region command inilizetion

    static MyPackageFactoryPool()
    {
        LoadAllCommand();
    }

    /// <summary>
    /// 加载所有继承了 MyPackage 的命令
    /// </summary>
    internal static void LoadAllCommand()
    {
        var packets = typeof(MyPackage).Assembly.GetTypes()
            .Where(t => typeof(MyPackage).IsAssignableFrom(t))
            .Where(t => !t.IsAbstract && t.IsClass)
            .Select(t => (MyPackage?)Activator.CreateInstance(t));

        using var enumerator = packets.GetEnumerator();
        while (enumerator.MoveNext())
        {
            if (enumerator.Current != null)
                CommandTypes.TryAdd(enumerator.Current.GetType(), enumerator.Current.Key);
        }
    }

    #endregion

    public MyPackageFactoryPool()
    {
        var commands = CommandTypes;

        _packetFactories = new IPackageFactory[commands.Count + 1];

        foreach (var command in commands)
        {
            var genericType = typeof(MyPackageFactory<>).MakeGenericType(command.Key);

            if (Activator.CreateInstance(genericType) is not IPackageFactory packetFactory)
                continue;

            _packetFactories[(int)command.Value] = packetFactory;
        }
    }

    public IPackageFactory Get(MyCommand command)
    {
        return _packetFactories[(byte)command];
    }

    public IPackageFactory Get<TPackage>()
    {
        var key = CommandTypes[typeof(TPackage)];

        return _packetFactories[(int)key];
    }
}

