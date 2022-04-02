namespace Qwirkle.WebApi.Shared;

public class UserInfo
{
    public bool IsAuthenticated { get; init; }
    public string UserName { get; init; }
    public Dictionary<string, string> ExposedClaims { get; init; }
}