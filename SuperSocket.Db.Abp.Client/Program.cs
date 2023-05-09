using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SuperSocket.Client;
using SuperSocket.Client.Command;
using SuperSocket.Db.Abp.Client;
using SuperSocket.Db.Abp.Core;
using System.Net;
using System.Net.Sockets;

var services = new ServiceCollection();

services.AddLogging(logging => logging.AddDebug().AddConsole());

services.AddCommandClient<MyCommand, MyPackage>(option =>
{
    option.UseClient<MyClient>();
    option.UsePackageEncoder<MyPackageEncoder>();
    option.UsePipelineFilter<MyPipeLineFilter>();
    option.UseCommand(options => options.AddCommandAssembly(typeof(LoginAck).Assembly));
});

var provider = services.BuildServiceProvider();

var client = provider.GetRequiredService<MyClient>();

await client.ConnectAsync(new DnsEndPoint("127.0.0.1", 4040, AddressFamily.InterNetwork), CancellationToken.None);

var loginResponse = await client.LoginAsync(new LoginPackage
{
    Username = "wujun",
    Password = "passwssssssord",
});

client.Logger.LogInformation($"登录结果:{loginResponse}");

Console.ReadKey();










//var easyClient = new EasyClient<MyPackage, MyPackage>(new MyPipeLineFilter { Decoder = new MyPackageDecoder(new MyPackageFactoryPool()) }, new MyPackageEncoder()).AsClient();

//await easyClient.ConnectAsync(new DnsEndPoint("127.0.0.1", 4040, System.Net.Sockets.AddressFamily.InterNetwork), CancellationToken.None);

//await easyClient.SendAsync(new RegisterPackage
//{
//    Username = "wujun",
//    Password = "passwssssssord",
//    Email = "8400684@qq.com"
//});

//var registerResponse = await easyClient.ReceiveAsync();

//Console.WriteLine($"注册结果:{registerResponse}");

//await easyClient.SendAsync(new LoginPackage
//{
//    Username = "wujun",
//    Password = "passwssssssord",
//});

//var loginResponse = await easyClient.ReceiveAsync();

//Console.WriteLine($"登录结果:{loginResponse}");

//Console.ReadKey();