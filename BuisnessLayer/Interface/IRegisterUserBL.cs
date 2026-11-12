
using RepositoryLayer.Entity;

namespace BuisnessLayer.Interface;

public interface IRegisterUserBL
{
    Task<UserEntity?> GetUserByEmailAsync(string email);

    Task<UserEntity?> RegisterUserAsync(UserEntity user);

    Task<int?> GetUserIdByEmailAsync(string email);

    Task<bool> UpdatePasswordHashAsync(
        string email,
        string passwordHash);
}
