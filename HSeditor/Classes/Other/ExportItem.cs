using HSeditor.Classes.Item_Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSeditor.Classes.Other
{
    public class ExportItem
    {
        public string Name { get; set; }
        public int ID { get; set; }
        public Rarity Rarity { get; set; }
        public Slot Slot { get; set; }
        public WeaponType WeaponType { get; set; }
        public Stats Stats { get; set; }

        public ExportItem(string name, int iD, Rarity rarity, Slot slot, WeaponType weaponType, Stats stats)
        {
            Name = name;
            ID = iD;
            Rarity = rarity;
            Slot = slot;
            WeaponType = weaponType;
            Stats = stats;
        }
    }
}
