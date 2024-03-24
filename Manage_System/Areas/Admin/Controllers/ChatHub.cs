using Manage_System.models;
using Manage_System.Service;
using Microsoft.AspNetCore.SignalR;

namespace Manage_System.Areas.Admin.Controllers
{
    public class ChatHub : Hub
    {
        private readonly IChatService _chatService;

        public ChatHub(IChatService chatService)
        {
            _chatService = chatService;
        }

        public async Task SendMessage(Message message)
        {
            _chatService.SendMessage(message);
            await Clients.All.SendAsync("ReceiveMessage", message.Sender, message.Content);
        }
    }
}
