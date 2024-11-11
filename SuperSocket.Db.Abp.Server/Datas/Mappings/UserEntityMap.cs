using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SuperSocket.Db.Abp.Server.Entitys;

namespace SuperSocket.Db.Abp.Server.Data.Mappings;

public sealed class UserEntityMap : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.ToTable("user_table")
                   .HasKey(m => m.Id);

        builder.HasIndex(m => m.Id);
    }
}
