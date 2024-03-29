using Manage_System.models;

namespace Manage_System.ModelViews
{
    public class GuesModelDisplay
    {
        public IEnumerable<Contribution> Contributions { get; set; }
        public IEnumerable<Magazine> Magazines { get; set; }
        public int magazineId { get; set; } = 0;
    }
}
