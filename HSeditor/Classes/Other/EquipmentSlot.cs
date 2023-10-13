using System.Windows;
using System.Windows.Media;

namespace HSeditor.Classes.Other
{
    public class EquipmentSlot
    {
        public int ID { get; private set; }
        public Slot Slot { get; private set; }
        public Item Item { get; set; }
        public string Border { get; private set; }
        public dynamic Background { get; private set; }
        public string Sprite { get; private set; }
        public bool Mercenary { get; set; }
        public Item temp { get; set; }

        public EquipmentSlot(int ID, Slot Slot, Item Item)
        {
            this.ID = ID;
            this.Slot = Slot;
            this.Item = Item;
            this.Mercenary = false;
            this.SetItem(Item);
        }

        public void SetItem(Item? item)
        {
            this.Item = item;
            this.Border = item == null ? "#FF483D85" : item.Rarity.Color;
            this.Background = item == null ? "#FF171519" : new LinearGradientBrush((Color)ColorConverter.ConvertFromString(item.Rarity.BackgroundColor2), (Color)ColorConverter.ConvertFromString(item.Rarity.BackgroundColor), new Point(1, 0.7), new Point(0.4, 0.3));

            this.Sprite = item == null ? this.Slot.Sprite : item.Sprite;
        }

        public void SetTempItem(Item item)
        {
            this.temp = this.Item;
            this.SetItem(item);
        }

        public void ResetTempItem()
        {
            this.SetItem(temp);
            this.temp = null;
        }
    }
}
