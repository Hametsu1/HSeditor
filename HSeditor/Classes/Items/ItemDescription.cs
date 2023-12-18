using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSeditor.Classes.Items
{
    public class ItemDescription
    {
        public string Effects { get; set; }
        public string Ability { get; set; }
        public string Defense { get; set; }
        public string Damage { get; set; }
        public string APS { get; set; }
        public string Block { get; set; }
        public List<Stat> Stats { get; set; }
        public string Lore { get; set; }

        public ItemDescription()
        {
            this.Effects = null;
            this.Stats = null;
            this.Lore = null;
            this.Ability = null;
            this.Defense = null;
            this.Damage = null;
            this.APS = null;
            this.Block = null;
        }
    }

    public class DescriptionHandler
    {
        public enum Type { Defense = 0, Damage = 1, APS = 2, Block = 3, Stats = 4, Passive = 5, Ability = 6, Lore = 7, Tier = 8, Hands = 9 }

        public DescriptionHandler()
        {
            this.GetItemDescriptions();
        }

        private void GetItemDescriptions()
        {
            var result = MainWindow.INSTANCE.iniDB.Read("SELECT * FROM TooltipInfo");
            List<Item> uniques = MainWindow.INSTANCE.ItemHandler.Unique;
            List<Item> runewords = MainWindow.INSTANCE.ItemHandler.Runewords;

            while (result.Read())
            {
                int itemid = result.GetInt32("itemid");
                int slotid = result.GetInt32("slotid");
                int weapontypeid = result.GetInt32("weapontypeid");
                Type type = (Type)result.GetInt32("typeid");
                string text = result.GetString("description");

                List<Item> matchingItems = new List<Item>();
                if (slotid == -1)
                    matchingItems = runewords.FindAll(o => o.RunewordID == itemid);
                else
                    matchingItems = new List<Item> { uniques.Find(o => o.ID == itemid && o.Slot.ID == slotid && o.WeaponType.ID == weapontypeid) };

                if (matchingItems.Count == 0 || matchingItems[0] == null) continue;

                ItemDescription desc = matchingItems[0].ItemDescription;
                if (desc == null) desc = new ItemDescription();

                switch (type)
                {
                    case Type.Defense:
                        desc.Defense = text;
                        break;
                    case Type.Damage:
                        desc.Damage = text;
                        break;
                    case Type.APS:
                        desc.APS = text;
                        break;
                    case Type.Block:
                        desc.Block = text;
                        break;
                    case Type.Stats:
                        desc.Stats = MainWindow.INSTANCE.StatHandler.GetStatList(text);
                        break;
                    case Type.Passive:
                        string allPassives = "";
                        text.Split('|').ToList().ForEach(passive =>
                        {
                            bool isRange = Char.IsNumber(passive[0]);
                            if (isRange)
                            {
                                string range = passive.Split(new char[] { ' ' }, 2)[0];
                                if (range.Trim('-').Contains("-"))
                                {
                                    passive = passive.Replace(range + " ", "");
                                    passive += $" [{range}]";
                                }
                            }
                            allPassives += passive + "\r\n";
                        });
                        desc.Effects = allPassives.TrimEnd('\r', '\n');
                        break;
                    case Type.Ability:
                        string allAbilities = "";
                        text.Split('|').ToList().ForEach(ability =>
                        {
                            bool isRange2 = Char.IsNumber(ability[0]);
                            if (isRange2)
                            {
                                string range = ability.Split(new char[] { ' ' }, 2)[0];
                                if (range.Trim('-').Contains("-"))
                                {
                                    ability = ability.Replace(range + " ", "");
                                    ability += $" [{range}]";
                                }
                            }
                            allAbilities += ability + "\r\n";
                        });
                        desc.Ability = allAbilities.TrimEnd('\r', '\n');
                        break;
                    case Type.Lore:
                        desc.Lore = text.Replace("|", "\r\n");
                        break;
                }
                matchingItems.ForEach(o => o.ItemDescription = desc);
            }
            result.Close();
        }
    }
}
