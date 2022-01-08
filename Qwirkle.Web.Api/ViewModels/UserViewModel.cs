namespace Qwirkle.Web.Api.ViewModels;

public class UserViewModel
{
    public string Pseudo { get; init; }
    public string Password { get; init; }
    public string Firstname { get; init; }
    public string Lastname { get; init; }
    public string Email { get; init; }


    public User ToUser() => new(Pseudo, Email, Firstname, Lastname);
}