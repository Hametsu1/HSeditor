using HSeditor.Classes.Other;
using System.Collections.Generic;
using System.ComponentModel;

namespace HSeditor.Classes.Filter.Item
{
    public class ItemFilter
    {
        public BindingList<RarityFilter> RarityFilter { get; set; }
        public Slot Slot { get; set; }
        public WeaponType WeaponType { get; set; }
        public string Name { get; set; }

        public ItemFilter()
        {
            this.RarityFilter = new BindingList<RarityFilter>();
            foreach (Rarity rarity in MainWindow.INSTANCE.RarityHandler.RaritiesFiltered)
                this.RarityFilter.Add(new RarityFilter(rarity, false));
        }

        public bool ContainsRarity(Rarity rarity)
        {
            if (this.GetFilteredRarities().Contains(rarity))
                return true;
            return false;
        }

        public List<Rarity> GetFilteredRarities()
        {
            List<Rarity> rarities = new List<Rarity>();
            foreach (RarityFilter rarity in this.RarityFilter)
                if (rarity.Selected && rarity.Enabled)
                    rarities.Add(rarity.Rarity);
            return rarities;
        }
    }
}
