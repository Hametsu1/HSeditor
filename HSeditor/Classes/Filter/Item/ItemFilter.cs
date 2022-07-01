using HSeditor.Classes.Other;
using System.Collections.Generic;
using System.ComponentModel;

namespace HSeditor.Classes.Filter.Item
{
    public class ItemFilter
    {
        public BindingList<RarityFilter> RarityFilter { get; set; }
        public BindingList<TierFilter> TierFilter { get; set; }
        public BindingList<StatFilter> StatFilter { get; set; }
        public Slot Slot { get; set; }
        public WeaponType WeaponType { get; set; }
        public string Name { get; set; }

        public ItemFilter()
        {
            this.RarityFilter = new BindingList<RarityFilter>();
            foreach (Rarity rarity in MainWindow.INSTANCE.RarityHandler.RaritiesFiltered)
                this.RarityFilter.Add(new RarityFilter(rarity, false));

            this.TierFilter = new BindingList<TierFilter>();
            foreach (Tier tier in MainWindow.INSTANCE.StatHandler.Tiers)
                this.TierFilter.Add(new TierFilter(tier, false));

            this.StatFilter = new BindingList<StatFilter>();
            foreach (Stat stat in MainWindow.INSTANCE.StatHandler.GetFilteredStats())
                this.StatFilter.Add(new StatFilter(stat, false));
        }

        public List<Stat> GetFilteredStats()
        {
            List<Stat> statList = new List<Stat>();
            foreach (StatFilter stat in this.StatFilter)
                if (stat.Selected)
                    statList.Add(stat.Stat);
            return statList;
        }

        public bool ContainsRarity(Rarity rarity)
        {
            if (this.GetFilteredRarities().Contains(rarity))
                return true;
            return false;
        }

        public bool ContainsTier(Tier tier)
        {
            if (this.GetFilteredTiers().Contains(tier))
                return true;
            return false;
        }

        public bool ContainsStat(Stat stat)
        {
            foreach (Stat Stat in this.GetFilteredStats())
                if (Stat.Name == stat.Name)
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

        public List<Tier> GetFilteredTiers()
        {
            List<Tier> tiers = new List<Tier>();
            foreach (TierFilter tier in this.TierFilter)
                if (tier.Selected)
                    tiers.Add(tier.Tier);
            return tiers;
        }
    }
}
