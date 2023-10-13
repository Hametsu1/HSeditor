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
        public string Stats { get; set; }
        public string Lore { get; set; }

        public ItemDescription()
        {
            this.Effects = null;
            this.Stats = null;
            this.Lore = null;
        }
    }

    public class DescriptionHandler
    {
        public enum Type { Effect = 0, Stat = 1, Lore = 2 }

        public DescriptionHandler()
        {
            this.GetItemDescriptions();
        }

        private void GetItemDescriptions()
        {

            var result = MainWindow.INSTANCE.iniDB.Read("SELECT * FROM TooltipInfo");

            while (result.Read())
            {
                Item item = MainWindow.INSTANCE.ItemHandler.AllItems.Find(o => o.ID == result.GetInt32("itemid") && o.Slot.ID == result.GetInt32("slotid") && o.WeaponType.ID == result.GetInt32("weapontypeid"));
                if (item == null) continue;
                Type type = (Type)result.GetInt32("typeid");
                string text = result.GetString("description");
                if (item.ItemDescription == null) item.ItemDescription = new ItemDescription();
                switch (type)
                {
                    case Type.Effect:
                        item.ItemDescription.Effects = text.Replace("|", "\r\n");
                        break;
                    case Type.Stat:
                        item.ItemDescription.Stats = text.Replace("|", "\r\n");
                        break;
                    case Type.Lore:
                        item.ItemDescription.Lore = text.Replace("|", "\r\n");
                        break;
                }
            }
            result.Close();
        }
    }
}
