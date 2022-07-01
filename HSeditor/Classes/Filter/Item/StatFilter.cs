namespace HSeditor.Classes.Filter.Item
{
    public class StatFilter
    {
        public Stat Stat { get; private set; }
        public bool Selected { get; set; }

        public StatFilter(Stat Stat, bool Selected)
        {
            this.Stat = Stat;
            this.Selected = Selected;
        }
    }
}
