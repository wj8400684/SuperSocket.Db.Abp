using SuperSocket.Client;
using SuperSocket.Db.Abp.Core;
using System.Net;

var easyClient = new EasyClient<MyPackage, MyPackage>(new MyPipeLineFilter { Decoder = new MyPackageDecoder(new MyPackageFactoryPool()) }, new MyPackageEncoder()).AsClient();

await easyClient.ConnectAsync(new DnsEndPoint("127.0.0.1", 4040, System.Net.Sockets.AddressFamily.InterNetwork), CancellationToken.None);

await easyClient.SendAsync(new RegisterPackage
{
    Username = "wujun",
    Password = "passwssssssord",
    Email = "8400684@qq.com"
});

var registerResponse = await easyClient.ReceiveAsync();

Console.WriteLine($"注册结果:{registerResponse}");

await easyClient.SendAsync(new LoginPackage
{
    Username = "wujun",
    Password = "passwssssssord",
});

var loginResponse = await easyClient.ReceiveAsync();

Console.WriteLine($"登录结果:{loginResponse}");

Console.ReadKey();