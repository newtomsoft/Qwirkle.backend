namespace Qwirkle.WebApi.Shared;

[Serializable]
public class RegisterModel
{
    [Required]
    public string UserName { get; set; }

    [Required, DataType(DataType.EmailAddress), EmailAddress]
    public string Email { get; set; }

    [Required, DataType(DataType.Password), MinLength(UserService.PassWordMinLength)]
    public string Password { get; set; }

    [Required, DataType(DataType.Password), MinLength(UserService.PassWordMinLength), Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
    public string PasswordConfirm { get; set; }

    public string Firstname { get; set; }

    public string Lastname { get; set; }

    public bool SignInPersistent { get; set; }


    public User ToUser() => new(UserName, Email, Firstname, Lastname);
}