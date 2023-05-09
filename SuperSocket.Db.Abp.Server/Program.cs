using SuperSocket;
using SuperSocket.Db.Abp.Server;
using SuperSocket.Command;
using SuperSocket.Db.Abp.Server.Commands;
using SuperSocket.ProtoBase;
using Microsoft.EntityFrameworkCore;
using SuperSocket.Db.Abp.Server.Data;
using EntityFrameworkCore.UnitOfWork.Extensions;
using SuperSocket.IOCPTcpChannelCreatorFactory;

var builder = WebApplication.CreateBuilder(args);

builder.Host.AsSuperSocketHostBuilder<MyPackage, MyPipeLineFilter>()
            .UseHostedService<MyAppServer>()
            .UseSession<MyAppSession>()
            .UsePackageDecoder<MyPackageDecoder>()
            .UseCommand(options => options.AddCommandAssembly(typeof(LoginCommand).Assembly))
            .UseClearIdleSession()
            .UseIOCPTcpChannelCreatorFactory()
            .UseInProcSessionContainer()
            .AsMinimalApiHostBuilder()
            .ConfigureHostBuilder();

builder.Services.AddSingleton<IPackageFactoryPool, MyPackageFactoryPool>();

builder.Services.AddSingleton<IPackageEncoder<MyPackage>, MyPackageEncoder>();

builder.Services.AddDbContextPool<DbContextFactory>(options => options.UseSqlite("Data Source=supersocket_mq.db;"));

builder.Services.AddScoped<DbContext, DbContextFactory>()
                .AddUnitOfWork()
                .AddUnitOfWork<DbContextFactory>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
