namespace Qwirkle.WebApi.Client.Blazor.Tests.Arrange;

public class RegisterTestsInput
{
    public RegisterModel RegisterModel { get; }
    public string InputSelectorInError { get; }

    public RegisterTestsInput(string inputSelectorInError, RegisterModel registerModel)
    {
        RegisterModel = registerModel;
        InputSelectorInError = inputSelectorInError;
    }

    public override string ToString()
    {
        var error = string.IsNullOrEmpty(InputSelectorInError) ? "no error" : InputSelectorInError;
        var userName = RegisterModel.UserName ?? "is null";
        var email = RegisterModel.Email ?? "is null";
        var password = RegisterModel.Password ?? "is null";
        var passwordConfirm = RegisterModel.PasswordConfirm ?? "is null";
        return $"{error} for UserName:{userName}, email:{email}, password:{password}, confirmPassword:{passwordConfirm}";
    }
}