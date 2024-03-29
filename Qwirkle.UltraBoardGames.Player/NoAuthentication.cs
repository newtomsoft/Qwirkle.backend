﻿namespace Qwirkle.UltraBoardGames.Player;

internal class NoAuthentication : IAuthentication
{
    public Task<bool> RegisterAsync(User user, string password) => throw new NotSupportedException();
    public Task<bool> RegisterGuestAsync() => throw new NotSupportedException();
    public Task LogoutOutAsync() => throw new NotSupportedException();
    public Task<bool> LoginAsync(string pseudo, string password, bool isRemember) => throw new NotSupportedException();
    public bool IsBot(int userId) => false;
}
