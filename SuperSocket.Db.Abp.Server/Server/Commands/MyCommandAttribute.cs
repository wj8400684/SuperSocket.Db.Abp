using SuperSocket.Command;

namespace SuperSocket.Db.Abp.Server.Commands;

public sealed class MyCommandAttribute : CommandAttribute
{
    public MyCommandAttribute(MyCommand key)
    {
        Key = (byte)key;
    }
}
