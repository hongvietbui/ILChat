using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using ILChat.Entities;
using ILChat.Repositories.IRepositories;

namespace ILChat.Services;

public class UserService(IUnitOfWork unitOfWork, IMapper mapper) : ILChat.UserService.UserServiceBase
{
    public override async Task<GetUserResponse> GetUser(GetUserRequest request, ServerCallContext context)
    {
        var user = await unitOfWork.Repository<User>().FirstOrDefaultAsync(filter: u => (u.FirstName +" "+ u.LastName).Contains(request.Query) || u.Username.Contains(request.Query));
        
        if (user == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "User not found"));
        }

        return new GetUserResponse
        {
            Meta = new BaseResponse
            {
                Timestamp = Timestamp.FromDateTime(DateTime.Now.ToUniversalTime()),
                Message = "Get user successfully"
            },
            Data = mapper.Map<GetUserOutput>(user)
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
        
        var userCreated = mapper.Map<GetUserOutput>(user);

        return new CreateUserResponse
        {
            Meta = new BaseResponse
            {
                Timestamp = Timestamp.FromDateTime(DateTime.Now.ToUniversalTime()),
                Message = "Create user successfully"
            },
            Data = userCreated
        };
    }

    public override async Task<StringBaseResponse> DeleteUser(DeleteUserRequest request, ServerCallContext context)
    {
        var isValid = Guid.TryParse(request.Data, out var userId);
        
        if(!isValid)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid user id"));
        
        var isUserExisted = await unitOfWork.Repository<User>()
            .AnyAsync(u => string.Equals(u.Id.ToString(), request.Data));
        
        if (!isUserExisted)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "User not found"));
        }

        await unitOfWork.Repository<User>().DeleteByIdAsync(userId);
        await unitOfWork.SaveChangesAsync();
        
        return new StringBaseResponse
        {
            Data = "",
            Meta = new BaseResponse
            {
                Message = "Delete user successfully",
                Timestamp = Timestamp.FromDateTime(DateTime.Now.ToUniversalTime())
            }
        };
    }

    public async override Task<StringBaseResponse> UpdateUser(UpdateUserRequest request, ServerCallContext context)
    {
        var isUserExisted = await unitOfWork.Repository<User>()
            .AnyAsync(u => string.Equals(u.Id.ToString(), request.Data));
        
        if (!isUserExisted)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "User not found"));
        }

        var user = await unitOfWork.Repository<User>().GetByIdAsync(request.Data);
        user.FirstName = request.Data.FirstName;
        user.LastName = request.Data.LastName;
        user.Username = request.Data.Username;
        user.Email = request.Data.Email;
        unitOfWork.Repository<User>().Update(user);
        
        return new StringBaseResponse
        {
            Data = "",
            Meta = new BaseResponse
            {
                Message = "Update user successfully",
                Timestamp = Timestamp.FromDateTime(DateTime.Now.ToUniversalTime())
            }
        };
    }
}