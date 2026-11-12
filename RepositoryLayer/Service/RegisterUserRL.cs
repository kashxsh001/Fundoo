
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.context;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;

namespace RepositoryLayer.Service;

public class RegisterUserRL : IRegisterUserRL
{
    private readonly Fundoo2Context _context;

    public RegisterUserRL(Fundoo2Context context)
    {
        _context = context;
    }

    public async Task<UserEntity?> GetUserByEmailAsync(
        string email)
    {
        string normalizedEmail = email.Trim().ToLowerInvariant();

        return await _context.Users
            .FirstOrDefaultAsync(user =>user.Email.ToLower() == normalizedEmail);
    }

    public async Task<UserEntity?> RegisterUserAsync(
        UserEntity user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<int?> GetUserIdByEmailAsync(string email)
    {
        string normalizedEmail = email.Trim().ToLowerInvariant();

        return await _context.Users
            .Where(user =>
                user.Email.ToLower() == normalizedEmail)
            .Select(user => (int?)user.User_ID)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> UpdatePasswordHashAsync(string email,string passwordHash)
    {
        string normalizedEmail = email.Trim().ToLowerInvariant();

        var user = await _context.Users
            .FirstOrDefaultAsync(user =>user.Email.ToLower() == normalizedEmail);

        if (user == null)
        {
            return false;
        }

        // The Handler hashes the password before calling this method.
        user.Password = passwordHash;

        await _context.SaveChangesAsync();

        return true;
    }
}
