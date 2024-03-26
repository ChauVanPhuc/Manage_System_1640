using Manage_System.models;

namespace Manage_System.ModelViews
{
    public class RulesModelView
    {
        public IEnumerable<Rule> Rules { get; set; }
        public IEnumerable<Security> Security { get; set; }
    }
}
