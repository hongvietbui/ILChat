using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using AutoMapper;
using Grpc.Core;
using ILChat.Entities;
using ILChat.Repositories.IRepositories;
using Microsoft.AspNetCore.Authorization;

namespace ILChat.Services;

public class UserService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration config, HttpClient httpClient)
    : ILChat.UserService.UserServiceBase
{
    [Authorize]
    public override async Task<GetUserResponse> GetUser(GetUserRequest request, ServerCallContext context)
    {
        var user = await unitOfWork.Repository<User>().FirstOrDefaultAsync(filter: u =>
            (u.FirstName + " " + u.LastName).Contains(request.Query) || u.Username.Contains(request.Query));

        if (user == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "User not found"));
        }

        return mapper.Map<GetUserResponse>(user);
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
            username = request.Username,
            email = request.Email,
            enabled = true,
            firstName = request.FirstName,
            credentials = new[]
            {
                new
                {
                    type = "password",
                    value = request.Password,
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
            throw new RpcException(new Status(StatusCode.Internal, error));
        }

        return new StringBaseResponse
        {
            Data = "Create user successfully",
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
        var isValid = Guid.TryParse(request.Id, out var userId);

        if (!isValid)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid user id"));

        var isUserExisted = await unitOfWork.Repository<User>()
            .AnyAsync(u => string.Equals(u.Id.ToString(), request));

        if (!isUserExisted)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "User not found"));
        }

        await unitOfWork.Repository<User>().DeleteByIdAsync(userId);
        await unitOfWork.SaveChangesAsync();

        return new StringBaseResponse
        {
            Data = "Delete user successfully"
        };
    }

    public override async Task<StringBaseResponse> UpdateUser(UpdateUserRequest request, ServerCallContext context)
    {
        var isUserExisted = await unitOfWork.Repository<User>()
            .AnyAsync(u => string.Equals(u.Id.ToString(), request));

        if (!isUserExisted)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "User not found"));
        }

        var user = await unitOfWork.Repository<User>().GetByIdAsync(request);
        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.Username = request.Username;
        user.Email = request.Email;
        unitOfWork.Repository<User>().Update(user);

        return new StringBaseResponse
        {
            Data = "Update user successfully"
        };
    }
}