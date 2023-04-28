namespace SuperSocket.Db.Abp.Server.Entitys;

public sealed class UserEntity
{
    public Guid Id { get; set; }

    public string Username { get; set; } = default!;

    public string Password { get; set; } = default!;

    public string IpAddress { get; set; } = default!;

    public string? Email { get; set; }

    public DateTime CreateTime { get; set; } = DateTime.UtcNow;
}
