using System;
using System.Collections.Generic;

namespace Manage_System.models;

public partial class Message
{
    public int Id { get; set; }

    public int? Sender { get; set; }

    public int? Receiver { get; set; }

    public string? Content { get; set; }

    public DateTime? SentAt { get; set; }

    public virtual User? ReceiverNavigation { get; set; }

    public virtual User? SenderNavigation { get; set; }
}
