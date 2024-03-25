using Manage_System.models;

namespace Manage_System.Areas.Coordinator.ModelView
{
    public class ChatViewModel
    {
        public string RecipientName { get; set; }
        public int revId { get; set; }
        public int sendvId { get; set; }
        public List<Message> MyMessages { get; set; }
        public List<Message> OtherMessages { get; set; }
        public Message LastMessage { get; set; }
    }
}
