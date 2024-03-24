using Manage_System.Hubs;
using Manage_System.models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Manage_System.Controllers
{
    public class ChatController : Controller
    {
        private readonly ManageSystem1640Context db;
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatController(ManageSystem1640Context dataContext, IHubContext<ChatHub> hubContext)
        {
            db = dataContext;
            _hubContext = hubContext;
        }
        public async Task<IActionResult> SendMessage(string to, string text)
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return StatusCode(500);
                }
                var account = HttpContext.Session.GetString("AccountId");

                var user = db.Users.AsNoTracking().SingleOrDefault(x => x.Id == int.Parse(account));
                var recipient = await db.Users.SingleOrDefaultAsync(x => x.Id == int.Parse(to));

                Message message = new Message()
                {
                    Sender = user.Id,
                    Receiver = recipient.Id,
                    Content = text,
                    SentAt = DateTime.Now
                };
                await db.AddAsync(message);
                await db.SaveChangesAsync();

                string connectionId = ChatHub.UsernameConnectionId[to];

                await _hubContext.Clients.Clients(connectionId).SendAsync("RecieveMessage", message.Content, message.SentAt);

                return Ok();
            }
            catch (Exception)
            {

                return Ok();
            }
            
        }
    }
}
