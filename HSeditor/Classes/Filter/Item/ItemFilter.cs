using HSeditor.Classes.Items;
using HSeditor.Classes.Other;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace HSeditor.Classes.Filter.Item
{
    public class ItemFilter
    {
        public List<RarityFilter> RarityFilter { get; set; }
        public List<StatFilter> StatFilter { get; set; }
        public Slot Slot { get; set; }
        public WeaponType WeaponType { get; set; }
        public string Name { get; set; }

        public ItemFilter()
        {
            this.RarityFilter = new List<RarityFilter>();
            foreach (Rarity rarity in MainWindow.INSTANCE.RarityHandler.RaritiesFiltered)
                this.RarityFilter.Add(new RarityFilter(rarity, false));

            this.StatFilter = new List<StatFilter>();
            MainWindow.INSTANCE.StatHandler.Stats.ForEach(stat => this.StatFilter.Add(new Item.StatFilter(stat, false)));
            this.StatFilter = this.StatFilter.OrderBy(o => o.Stat.Description).ThenBy(o => o.Stat.isPercentage).ToList();
        }

        public bool ContainsRarity(Rarity rarity)
        {
            if (this.GetFilteredRarities().Contains(rarity))
                return true;
            return false;
        }

        public bool ContainsStat(Stat stat)
        {
            return this.GetFilteredStats().Contains(stat);
        }

        public List<Rarity> GetFilteredRarities()
        {
            List<Rarity> rarities = new List<Rarity>();
            foreach (RarityFilter rarity in this.RarityFilter)
                if (rarity.Selected && rarity.Enabled)
                    rarities.Add(rarity.Rarity);
            return rarities;
        }

        public List<Stat> GetFilteredStats()
        {
            List<Stat> stats = new List<Stat>();
            this.StatFilter.ForEach(stat => { if (stat.Selected) stats.Add(stat.Stat); });
            return stats;
        }
    }
}
