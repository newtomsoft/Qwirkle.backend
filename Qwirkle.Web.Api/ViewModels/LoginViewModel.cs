namespace Qwirkle.Web.Api.ViewModels;

public class LoginViewModel
{
    public string UserName { get; set; }
    public string Password { get; set; }
    public bool IsRemember { get; set; }



    public UserDao ToUserDao()
    {
        return new ()
        {
            UserName = UserName,
            FirstName = "Toto",
            LastName = "Lolo"
        };
    }
}