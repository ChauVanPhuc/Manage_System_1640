using Manage_System.Areas.Coordinator.ModelView;
using Manage_System.models;
using Manage_System.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Manage_System.Areas.Coordinator.Controllers
{
    public class ChatController : Controller
    {

        private readonly IChatService _chatService;
        private readonly ManageSystem1640Context _db;
        public ChatController(IChatService chatService, ManageSystem1640Context db)
        {
            _chatService = chatService;
            _db = db;
        }


       

        
    }
}
