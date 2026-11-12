
using BuisnessLayer.Interface;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;

namespace BuisnessLayer.Service;

public class RegisterUserBL : IRegisterUserBL
{
    private readonly IRegisterUserRL _userRL;

    public RegisterUserBL(IRegisterUserRL userRL)
    {
        _userRL = userRL;
    }

    public Task<UserEntity?> GetUserByEmailAsync(string email)
    {
        return _userRL.GetUserByEmailAsync(email);
    }

    public Task<UserEntity?> RegisterUserAsync(UserEntity user)
    {
        return _userRL.RegisterUserAsync(user);
    }

    public Task<int?> GetUserIdByEmailAsync(string email)
    {
        return _userRL.GetUserIdByEmailAsync(email);
    }

    public Task<bool> UpdatePasswordHashAsync(
        string email,
        string passwordHash)
    {
        return _userRL.UpdatePasswordHashAsync(
            email,
            passwordHash);
    }
}
