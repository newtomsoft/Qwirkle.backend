namespace Qwirkle.WebApi.Client.Blazor.Services.Contracts.Apis;

public interface IApiUser
{
    Task Login(LoginModel loginModel);
    Task Register(RegisterModel registerModel);
    Task RegisterGuest();
    Task Logout();
    Task<UserInfo> GetUserInfo();
}