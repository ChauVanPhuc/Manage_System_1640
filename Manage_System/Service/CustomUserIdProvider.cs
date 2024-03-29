using Microsoft.AspNetCore.SignalR;

namespace Manage_System.Service
{
    public class CustomUserIdProvider : IUserIdProvider
    {
        public virtual string GetUserId(HubConnectionContext connection)
        {
            // Trả về AccountId từ claims nếu có.
            return connection.User?.FindFirst("AccountId")?.Value;
        }
    }

}
