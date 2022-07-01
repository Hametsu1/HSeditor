namespace HSeditor.Classes.Other
{
    public class EquipmentSlot
    {
        public Slot Slot { get; private set; }
        public Item Item { get; set; }
        public string Status { get; set; }
        public string Border { get; private set; }
        public string Sprite { get; private set; }
        public bool Mercenary { get; set; }
        public bool Invisible { get; set; }

        public EquipmentSlot(Slot Slot, Item Item)
        {
            this.Slot = Slot;
            this.Item = Item;
            this.Status = "#FF1B1D21";
            this.Border = "#FF483D85";
            this.Sprite = Slot.Sprite;
            this.Mercenary = false;
        }

        public void SetItem(Item? item)
        {
            this.Item = item;
            this.Border = item == null ? "#FF483D85" : item.Rarity.Color;
            this.Sprite = item == null ? Slot.Sprite : item.Sprite;
        }
    }
}
