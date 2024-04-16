using Manage_System.models;
using Microsoft.EntityFrameworkCore;

namespace Manage_System.Service
{
    public class IUserService
    {
        private readonly ManageSystem1640Context _context;

        public IUserService(ManageSystem1640Context context)
        {
            _context = context;
        }

        public async Task<User> FindByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<string> GeneratePasswordResetTokenAsync(User user)
        {
            
            user.PasswordResetToken = Guid.NewGuid().ToString();
            user.TokenExpiration = DateTime.UtcNow.AddMinutes(5); // Token
            await _context.SaveChangesAsync();
            return user.PasswordResetToken;
        }
    }
}
