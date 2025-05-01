using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using ILChat.Entities;
using ILChat.Repositories.IRepositories;

namespace ILChat.ServiceImpls;

public class UserServiceImpl(IUnitOfWork unitOfWork) : UserService.UserServiceBase
{
    public override async Task<GetUserResponse> GetUser(GetUserRequest request, ServerCallContext context)
    {
        var user = await unitOfWork.Repository<User>().FirstOrDefaultAsync(filter: u => (u.FirstName +" "+ u.LastName).Contains(request.Query) || u.Username.Contains(request.Query),
            include: null, disableTracking: true);
        
        if (user == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "User not found"));
        }

        return new GetUserResponse
        {
            Meta = new BaseResponse
            {
                Timestamp = Timestamp.FromDateTime(DateTime.Now.ToUniversalTime()),
                Message = "Get user successfully",
                Status = 200
            },
            Data = new GetUserOutput
            {
                Id = user.Id.ToString(),
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName
            }
        };
    }

    public async override Task<CreateUserResponse> CreateUser(CreateUserRequest request, ServerCallContext context)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = request.Data.Username,
            FirstName = request.Data.FirstName,
            LastName = request.Data.LastName,
            Email = request.Data.Email,
            Password = request.Data.Password
        };

        await unitOfWork.Repository<User>().AddAsync(user);
        await unitOfWork.SaveChangesAsync();
        
        var userCreated = new GetUserOutput
        {
            Id = user.Id.ToString(),
            Username = request.Data.Username,
            FirstName = request.Data.FirstName,
            LastName = request.Data.LastName
        };

        return new CreateUserResponse
        {
            Meta = new BaseResponse
            {
                Timestamp = Timestamp.FromDateTime(DateTime.Now.ToUniversalTime()),
                Message = "Create user successfully",
                Status = 200
            },
            Data = userCreated
        };
    }

    public async override Task<StringBaseResponse> DeleteUser(DeleteUserRequest request, ServerCallContext context)
    {
        return new StringBaseResponse
        {
            Data = "",
            Meta = new BaseResponse
            {
                Message = "Delete user successfully",
                Status = 200,
                Timestamp = Timestamp.FromDateTime(DateTime.Now.ToUniversalTime())
            }
        };
    }

    public async override Task<StringBaseResponse> UpdateUser(UpdateUserRequest request, ServerCallContext context)
    {
        return new StringBaseResponse
        {
            Data = "",
            Meta = new BaseResponse
            {
                Message = "Update user successfully",
                Status = 200,
                Timestamp = Timestamp.FromDateTime(DateTime.Now.ToUniversalTime())
            }
        };
    }
}