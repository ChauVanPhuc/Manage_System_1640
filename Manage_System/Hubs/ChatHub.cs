using Manage_System.models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Manage_System.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ManageSystem1640Context db;
        public static Dictionary<string, string> UsernameConnectionId = new();

        public ChatHub(ManageSystem1640Context dataContext)
        {
            db = dataContext;
        }
        public async Task<string> GetConnectionId(string username)
        {
            var user = await db.Users.SingleOrDefaultAsync(x => x.Id == int.Parse(username));

            if (UsernameConnectionId.ContainsKey(username))
            {
                UsernameConnectionId[username] = Context.ConnectionId;
            }
            else
            {
                UsernameConnectionId.Add(username, Context.ConnectionId);
            }

            return Context.ConnectionId;
        }
    }
}