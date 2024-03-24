using Manage_System.models;

namespace Manage_System.Service
{
    public class ChatService : IChatService
    {
        private readonly ManageSystem1640Context _context;

        public ChatService(ManageSystem1640Context context)
        {
            _context = context;
        }

        public void SendMessage(Message message)
        {
            _context.Messages.Add(message);
            _context.SaveChanges();
        }

        public IList<Message> GetMessages()
        {
            return _context.Messages.ToList();
        }
    }
}
