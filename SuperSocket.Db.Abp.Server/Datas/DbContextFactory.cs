using Microsoft.EntityFrameworkCore;

namespace SuperSocket.Db.Abp.Server.Data;

public sealed class DbContextFactory : DbContext
{
    public DbContextFactory(DbContextOptions<DbContextFactory> dbContextOptions)
            : base(dbContextOptions)
    {
    }

    /// <summary>
    /// 当模型创建时候
    /// </summary>
    /// <param name="modelBuilder"></param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //加载当前程序集所有的 Assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DbContextFactory).Assembly);
    }
}
