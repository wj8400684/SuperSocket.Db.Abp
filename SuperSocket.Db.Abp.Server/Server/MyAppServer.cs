using Microsoft.Extensions.Options;
using SuperSocket.Server;

namespace SuperSocket.Db.Abp.Server;

public sealed class MyAppServer : SuperSocketService<MyPackage>
{
    public MyAppServer(IServiceProvider serviceProvider, IOptions<ServerOptions> serverOptions) : base(serviceProvider, serverOptions)
    {
    }
}
