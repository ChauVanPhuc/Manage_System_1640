using Manage_System.models;

namespace Manage_System.Service
{
    public interface IChatService
    {
        void SendMessage(Message message);
        IList<Message> GetMessages();
    }
}
