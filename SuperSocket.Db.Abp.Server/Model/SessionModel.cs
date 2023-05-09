namespace SuperSocket.Db.Abp.Server.Model;

public sealed record SessionItem(string Id, bool IsLogined);

public sealed record SessionAllModel(IEnumerable<SessionItem> Sessions);
