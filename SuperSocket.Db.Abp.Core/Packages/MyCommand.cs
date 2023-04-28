namespace SuperSocket.Db.Abp.Core;

public enum MyCommand : byte
{
    None,
    Login,
    LoginAck,
    Register,
    RegisterAck,
}
