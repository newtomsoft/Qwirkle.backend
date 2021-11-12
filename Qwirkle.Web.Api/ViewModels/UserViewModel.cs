using Qwirkle.Core.Entities;

namespace Qwirkle.Web.Api.ViewModels;

public class UserViewModel
{
    public string Pseudo { get; set; }
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public string Password { get; set; }
    public bool IsRemember { get; set; }

    public UserDao ToUserDao()
    {
        return new ()
        {
            UserName = Pseudo,
            FirstName = Firstname,
            LastName = Lastname,
        };
    }

    public LoginViewModel ToLoginViewModel()
    {
        return new()
        {
            Pseudo = Pseudo,
            Password = Password,
            IsRemember = IsRemember,
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