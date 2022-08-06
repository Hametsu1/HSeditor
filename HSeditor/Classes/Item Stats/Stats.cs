using HSeditor.Classes.Other;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace HSeditor.Classes.Item_Stats
{
    public class Stats
    {
        public int? HandedType { get; set; }
        public string? Ability { get; set; }
        public string? Aura { get; set; }
        public int AbilityLevelMax { get; set; }
        public int AbilityLevelMin { get; set; }
        public int AuraLevelMin { get; set; }
        public int AuraLevelMax { get; set; }
        public int SocketsMax { get; set; }
        public int SocketsMin { get; set; }
        public string SocketString { get; set; }
        public double UpgradePrice { get; set; }
        public Tier Tier { get; set; }
        public int LevelRequirement { get; set; }
        public double Damage { get; set; }
        public string? APS { get; set; }
        public List<DamageType> DamageTypes { get; set; }
        public string AbilityString { get; set; }
        public string AuraString { get; set; }
        public Class Class { get; set; }
        public int PriorityCount { get; set; }
        public string UpgradePriceString { get; set; }
        public int VendorPrice { get; set; }

        public List<Stat> StatList { get; set; }

        public void Set(Item item)
        {
            this.AbilityString = this.Ability == null ? "empty" : this.AbilityLevelMin == this.AbilityLevelMax ? this.AbilityLevelMin == 0 ? $"{this.Ability}" : $"{this.Ability} [Level {this.AbilityLevelMin}]" : $"{this.Ability} [Level {this.AbilityLevelMin}-{this.AbilityLevelMax}]";
            this.AuraString = this.Aura == null ? "empty" : this.AuraLevelMin == this.AuraLevelMax ? this.AuraLevelMin == 0 ? $"{this.Aura}" : $"{this.Aura} [Level {this.AuraLevelMin}]" : $"{this.Aura} [Level {this.AuraLevelMin}-{this.AuraLevelMax}]";
            this.PriorityCount = 0;
            this.SocketString = this.SocketsMin == this.SocketsMax ? this.SocketsMin == 1 ? $"{this.SocketsMin} Socket" : $"{this.SocketsMin} Sockets" : $"{this.SocketsMin} - {this.SocketsMax} Sockets";
            if (item.Slot.ID != 3)
            {
                if (this.GetStat("INV_BLOCK") != null) { Stat stat = this.GetStat("INV_BLOCK"); stat.HasPriority = true; stat.Priority = 0; this.PriorityCount++; }
                if (this.GetStat("INV_ARMOR") != null) { Stat stat = this.GetStat("INV_ARMOR"); stat.HasPriority = true; stat.Priority = 1; this.PriorityCount++; }
                if (this.GetStat("INV_DODGE") != null) { Stat stat = this.GetStat("INV_DODGE"); stat.HasPriority = true; stat.Priority = 2; this.PriorityCount++; }
            }
            if (item.Slot.ID == 2)
                if (this.GetStat("INV_SPEED") != null) { Stat stat = this.GetStat("INV_SPEED"); stat.HasPriority = true; stat.Priority = 3; this.PriorityCount++; }
            this.StatList = this.StatList.OrderBy(o => o.Priority).ToList();
        }

        public bool Contains(Stat Stat)
        {
            foreach (Stat stat in this.StatList)
                if (stat.Name == Stat.Name)
                    return true;

            foreach (Stat stat in this.StatList)
                if (stat.DebugName == Stat.DebugName)
                    return true;
            return false;
        }

        public bool Contains(DamageType Damagetype)
        {
            foreach (DamageType damagetype in this.DamageTypes)
                if (damagetype.Name.ToLower().Replace(" ", String.Empty) == Damagetype.Name.ToLower().Replace(" ", String.Empty))
                    return true;
            return false;
        }

        public Stats DeepCopy()
        {
            Stats temp = (Stats)this.MemberwiseClone();
            temp.StatList = new List<Stat>();
            foreach (Stat stat in this.StatList)
                temp.StatList.Add(new Stat(stat.Name, stat.DebugName, stat.WíkiName, stat.Type, stat.Multiplier, stat.Priority, stat.HasPriority, stat.Value));
            return temp;
        }

        public Stat GetStat(string name)
        {
            foreach (Stat Stat in this.StatList)
                if (Stat.DebugName == name)
                    return Stat;
            return null;
        }

        public void Calculate(Item Item)
        {
            int level = Item.UpgradeLevel - 1;
            Stats def = MainWindow.INSTANCE.ItemHandler.GetDefaultStats(Item);

            double defaultValue = def.Damage * (Convert.ToDouble(Item.Quality) / 100.0);
            double increase = defaultValue * Double.Parse("0.15", CultureInfo.InvariantCulture);
            double newValue = defaultValue + level * increase;
            double bonus = 0.15 * (((def.Damage * 0.15) * level) + def.Damage);
            if (Item.Rarity.Name == "Heroic Set") newValue += bonus;
            Item.Stats.Damage = Math.Ceiling(newValue);


            foreach (Stat Stat in StatList)
            {
                if (Item.Quality == 100 && Item.UpgradeLevel == 1 && Item.Rarity.Name != "Heroic Set") { Stat.ChangeValue(def.GetStat(Stat.DebugName).Value); continue; }
                if (Stat.Multiplier == "null") continue;
                Stat.ChangeValue(this.CalculateStat(Item, Stat));
            }
        }

        private double CalculateStat(Item Item, Stat Stat)
        {
            Stats def = MainWindow.INSTANCE.ItemHandler.GetDefaultStats(Item);
            Stat defStat = def.GetStat(Stat.DebugName);
            int level = Item.UpgradeLevel - 1;
            double defaultValue = defStat.Value * (Convert.ToDouble(Item.Quality) / 100.0);
            double increase = Stat.Multiplier == "0" ? 0 : defaultValue * Double.Parse(Stat.Multiplier, CultureInfo.InvariantCulture);
            double newValue = defaultValue + level * increase;
            double bonus = 0.15 * (((defStat.Value * 0.15) * level) + defStat.Value);
            if (Item.Rarity.Name == "Heroic Set") newValue += bonus;
            return Math.Ceiling(newValue);
        }

        public Stats()
        {
            this.StatList = new List<Stat>();
        }
    }
}
