using SuperSocket.Db.Abp.Server.Entitys;

namespace SuperSocket.Db.Abp.Server.Commands;

[MyCommand(MyCommand.Register)]
public sealed class Register : MyAsyncRespCommand<RegisterPackage, RegisterRespPackage>
{
    public Register(IPackageFactoryPool packetFactoryPool)
        : base(packetFactoryPool)
    {
    }

    protected override async ValueTask<RegisterRespPackage> ExecuteAsync(MyAppSession session, RegisterPackage package, CancellationToken cancellationToken)
    {
        var response = CreateResponse();

        if (string.IsNullOrWhiteSpace(package.Username) || string.IsNullOrWhiteSpace(package.Password))
        {
            response.ErrorMessage = "参数错误";
            return response;
        }

        return await session.ExecuteDbAsync(async unitOfWork =>
        {
            var userRepository = unitOfWork.Repository<UserEntity>();

            if (await userRepository.AnyAsync(user => user.Username == package.Username, cancellationToken))
            {
                response.ErrorCode = MyErrorCode.UserExisted;
                response.ErrorMessage = "已经注册";
                return response;
            }

            await userRepository.AddAsync(
                cancellationToken: cancellationToken,
                entity: new UserEntity
                {
                    Username = package.Username,
                    Password = package.Password,
                    Email = package.Email,
                    IpAddress = session.RemoteAddress,
                });

            var result = await unitOfWork.SaveChangesAsync(cancellationToken: cancellationToken);

            if (result == 0)
            {
                response.ErrorCode = MyErrorCode.UnKnown;
                response.ErrorMessage = "注册失败";
            }
            else
            {
                response.SuccessFul = true;
            }

            return response;
        });
    }
}
