using SuperSocket.Db.Abp.Server.Entitys;

namespace SuperSocket.Db.Abp.Server.Commands;

[MyCommand(MyCommand.Login)]
public sealed class Login : MyAsyncRespCommand<LoginPackage, LoginRespPackage>
{
    public Login(IPackageFactoryPool packetFactoryPool)
        : base(packetFactoryPool)
    {
    }

    protected async override ValueTask<LoginRespPackage> ExecuteAsync(MyAppSession session, LoginPackage package, CancellationToken cancellationToken)
    {
        var response = CreateResponse();

        if (string.IsNullOrWhiteSpace(package.Username) || string.IsNullOrWhiteSpace(package.Password))
        {
            response.ErrorCode = MyErrorCode.ParameterError;
            response.ErrorMessage = "参数错误";
            return response;
        }

        return await session.ExecuteDbAsync(async unitOfWork =>
        {
            var userRepository = unitOfWork.Repository<UserEntity>();

            if (!await userRepository.AnyAsync(user => user.Username == package.Username && user.Password == package.Password, cancellationToken))
            {
                response.ErrorCode = MyErrorCode.UserOrPassError;
                response.ErrorMessage = "账号或密码错误";
                return response;
            }

            response.SuccessFul = true;

            return response;
        });
    }
}
