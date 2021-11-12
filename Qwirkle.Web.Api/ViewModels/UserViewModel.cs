namespace Qwirkle.Web.Api.ViewModels;

public class UserViewModel
{
    public string Pseudo { get; set; }
    public string Password { get; set; }
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public string Email { get; set; }

    public LoginViewModel ToLoginViewModel()
    {
        return new()
        {
            Pseudo = Pseudo,
            Password = Password,
        };
    }

    public User ToUser()
    {
        return new()
        {
            Pseudo = Pseudo,
            FirstName = Firstname,
            LastName = Lastname,

        };
    }
}