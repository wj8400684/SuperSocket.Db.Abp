using SuperSocket.Command;

namespace SuperSocket.Db.Abp.Server.Server.Commands;

/// <summary>
/// 登录过滤器
/// 只有登录之后的才能执行命令
/// </summary>
public sealed class MyLoginCommandFilter : AsyncCommandFilterAttribute
{
    public override ValueTask OnCommandExecutedAsync(CommandExecutingContext commandContext)
    {
        return ValueTask.CompletedTask;
    }

    public override ValueTask<bool> OnCommandExecutingAsync(CommandExecutingContext commandContext)
    {
        var session = (MyAppSession)commandContext.Session;

        return ValueTask.FromResult(session.IsLogined);
    }
}
