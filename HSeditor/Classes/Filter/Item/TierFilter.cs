using HSeditor.Classes.Other;

namespace HSeditor.Classes.Filter.Item
{
    public class TierFilter
    {
        public Tier Tier { get; private set; }
        public bool Selected { get; set; }

        public TierFilter(Tier Tier, bool Selected)
        {
            this.Tier = Tier;
            this.Selected = Selected;
        }
    }
}
