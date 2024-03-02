using System;
using System.Collections.Generic;

namespace Manage_System.models;

public partial class Role
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<User> Users { get; } = new List<User>();
}
