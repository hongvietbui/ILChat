using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using AutoMapper;
using Azure.Identity;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using ILChat.Entities;
using ILChat.Repositories.IRepositories;

namespace ILChat.Services;

public class UserService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration config, HttpClient httpClient)
    : ILChat.UserService.UserServiceBase
{
    public override async Task<GetUserResponse> GetUser(GetUserRequest request, ServerCallContext context)
    {
        var user = await unitOfWork.Repository<User>().FirstOrDefaultAsync(filter: u =>
            (u.FirstName + " " + u.LastName).Contains(request.Query) || u.Username.Contains(request.Query));

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

    public override async Task<StringBaseResponse> CreateUser(CreateUserRequest request, ServerCallContext context)
    {
        var realm = config["Keycloak:Realm"]!;
        var baseUrl = config["Keycloak:BaseUrl"]!;
        var clientToken = await GetClientTokenAsync();

        if (string.IsNullOrEmpty(clientToken))
            throw new RpcException(new Status(StatusCode.Internal, "Failed to get client token"));

        var userPayload = new
        {
            username = request.Data.Username,
            email = request.Data.Email,
            enabled = true,
            firstName = request.Data.FirstName,
            credentials = new[]
            {
                new
                {
                    type = "password",
                    value = request.Data.Password,
                    temporary = false
                }
            }
        };
        
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/admin/realms/{realm}/users");
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", clientToken);
        httpRequest.Content = new StringContent(JsonSerializer.Serialize(userPayload), Encoding.UTF8, "application/json");

        var response = await httpClient.SendAsync(httpRequest);
        
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new RpcException(new Status(StatusCode.Internal, $"Keycloak create user failed: {error}"));
        }

        return new StringBaseResponse
        {
            Meta =
            {
                Message = "Create user successfully",
                Timestamp = Timestamp.FromDateTime(DateTime.Now.ToUniversalTime())
            },
            Data = ""
        };
    }

    public async Task<string?> GetClientTokenAsync()
    {
        var baseUrl = config["Keycloak:BaseUrl"]!;
        var realm = config["Keycloak:Realm"]!;
        var clientId = config["Keycloak:ClientId"]!;
        var clientSecret = config["Keycloak:ClientSecret"]!;

        var parameters = new Dictionary<string, string>
        {
            { "client_id", clientId },
            { "client_secret", clientSecret },
            { "grant_type", "client_credentials" },
        };

        var tokenResponse = await httpClient.PostAsync(
            $"{baseUrl}/realms/{realm}/protocol/openid-connect/token",
            new FormUrlEncodedContent(parameters));
        
        // if (!tokenResponse.IsSuccessStatusCode) return null;
        
        if (!tokenResponse.IsSuccessStatusCode)
        {
            var errorContent = await tokenResponse.Content.ReadAsStringAsync();
            Console.WriteLine("Token request failed:");
            Console.WriteLine($"StatusCode: {tokenResponse.StatusCode}");
            Console.WriteLine($"Reason: {tokenResponse.ReasonPhrase}");
            Console.WriteLine($"Error Body: {errorContent}");
            return null;
        }
        
        var content = await tokenResponse.Content.ReadAsStringAsync();
        var tokenObj = JsonSerializer.Deserialize<JsonElement>(content);
        return tokenObj.GetProperty("access_token").GetString();
    }

    public override async Task<StringBaseResponse> DeleteUser(DeleteUserRequest request, ServerCallContext context)
    {
        var isValid = Guid.TryParse(request.Data, out var userId);

        if (!isValid)
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

    public override async Task<StringBaseResponse> UpdateUser(UpdateUserRequest request, ServerCallContext context)
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