namespace SuperSocket.Db.Abp.Core;

public interface IPacketAwaitable : IDisposable
{
    void Complete(MyPackage packet);

    void Fail(Exception exception);

    void Cancel();
}