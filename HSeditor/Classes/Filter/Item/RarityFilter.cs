namespace HSeditor.Classes.Filter.Item
{
    public class RarityFilter
    {
        public Rarity Rarity { get; private set; }
        public bool Selected { get; set; }
        public bool Enabled { get; set; }

        public RarityFilter(Rarity Rarity, bool Selected)
        {
            this.Rarity = Rarity;
            this.Selected = Selected;
            this.Enabled = this.Rarity.EditorID < 6 ? false : true;
        }
    }
}
