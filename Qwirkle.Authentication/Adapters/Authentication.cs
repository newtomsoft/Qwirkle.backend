using Microsoft.AspNetCore.Identity;
using Qwirkle.Core.Entities;
using Qwirkle.Core.Ports;
using Qwirkle.Infra.Repository.Dao;
using Qwirkle.Infra.Repository.DomainExtensionMethods;

namespace Qwirkle.Authentication.Adapters;

public class Authentication : IAuthentication
{
    private readonly UserManager<UserDao> _userManager;
    private readonly SignInManager<UserDao> _signInManager;
    private readonly IUserStore<UserDao> _userStore;

    public Authentication(SignInManager<UserDao> signInManager, UserManager<UserDao> userManager, IUserStore<UserDao> userStore)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _userStore = userStore;
    }

    public async Task<bool> RegisterAsync(User user, string password)
    {
        var userDao = user.ToUserDao();
        await _userStore.SetUserNameAsync(userDao, user.Pseudo, CancellationToken.None);
        var result = await _userManager.CreateAsync(userDao, password);
        return result.Succeeded;
    }
}