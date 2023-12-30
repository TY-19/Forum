using Forum.Application.Common.Interfaces;
using Forum.Application.Users.Dtos;
using MediatR;
using System.IdentityModel.Tokens.Jwt;

namespace Forum.Application.Users.Queries.Login;

public class LoginRequest : IRequest<LoginResponse>
{
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;
}

public class LoginRequestHandler(IUserManager userManager, IJwtHandler jwtHandler) : IRequestHandler<LoginRequest, LoginResponse>
{
    public async Task<LoginResponse> Handle(LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await userManager.GetUserByNameAsync(request.UserName, cancellationToken);
        if (user == null || !(await userManager.CheckPasswordAsync(user, request.Password, cancellationToken)).Succeeded)
            return new LoginResponse() { Succeeded = false, Message = "Invalid name or password" };

        string? token;
        try
        {
            token = new JwtSecurityTokenHandler().WriteToken(await jwtHandler.GetTokenAsync(user));
        }
        catch (Exception ex)
        {
            return new LoginResponse() { Succeeded = false, Message = ex.Message };
        }

        return new LoginResponse()
        {
            Succeeded = true,
            Message = "Login is complete",
            Token = token,
            UserName = user.UserName,
            UserProfileId = user.UserProfile.Id,
            Roles = await userManager.GetRolesAsync(user, cancellationToken)
        };
    }

}
