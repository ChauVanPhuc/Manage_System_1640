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
            // Sinh một token ngẫu nhiên, lưu vào database
            user.PasswordResetToken = Guid.NewGuid().ToString();
            user.TokenExpiration = DateTime.UtcNow.AddHours(1); // Token hết hạn sau 1 giờ
            await _context.SaveChangesAsync();
            return user.PasswordResetToken;
        }
    }
}
