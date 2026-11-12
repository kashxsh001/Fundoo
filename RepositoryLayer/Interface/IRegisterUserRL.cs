
using RepositoryLayer.Entity;

namespace RepositoryLayer.Interface;

public interface IRegisterUserRL
{
    Task<UserEntity?> GetUserByEmailAsync(string email);

    Task<UserEntity?> RegisterUserAsync(UserEntity user);

    Task<int?> GetUserIdByEmailAsync(string email);

    Task<bool> UpdatePasswordHashAsync(
        string email,
        string passwordHash);
}
