using Manage_System.models;
using System.ComponentModel.DataAnnotations;

namespace Manage_System.Areas.Coordinator.ModelView
{
    public class ChatModelView
    {

        public int? Sender { get; set; }

        public int? Receiver { get; set; }

        [Required(ErrorMessage = "Please, Enter Content")]
        public string? Content { get; set; }

        public DateTime? SentAt { get; set; }

        public virtual Contribution? ReceiverNavigation { get; set; }

        public virtual User? SenderNavigation { get; set; }
    }
}
