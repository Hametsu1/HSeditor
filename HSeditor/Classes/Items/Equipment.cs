using HSeditor.Classes.Other;
using System.Collections.Generic;

namespace HSeditor.Classes.Items
{
    public class Equipment
    {
        public EquipmentSlot Amulet { get; set; } = new EquipmentSlot(MainWindow.INSTANCE.SlotHandler.GetSlotFromName("Amulet"), null);
        public EquipmentSlot Helmet { get; set; } = new EquipmentSlot(MainWindow.INSTANCE.SlotHandler.GetSlotFromName("Helmet"), null);
        public EquipmentSlot Charm { get; set; } = new EquipmentSlot(MainWindow.INSTANCE.SlotHandler.GetSlotFromName("Charm"), null);
        public EquipmentSlot Weapon { get; set; } = new EquipmentSlot(MainWindow.INSTANCE.SlotHandler.GetSlotFromName("Weapon"), null);
        public EquipmentSlot Armor { get; set; } = new EquipmentSlot(MainWindow.INSTANCE.SlotHandler.GetSlotFromName("Armor"), null);
        public EquipmentSlot Shield { get; set; } = new EquipmentSlot(MainWindow.INSTANCE.SlotHandler.GetSlotFromName("Shield"), null);
        public EquipmentSlot Gloves { get; set; } = new EquipmentSlot(MainWindow.INSTANCE.SlotHandler.GetSlotFromName("Gloves"), null);
        public EquipmentSlot Belt { get; set; } = new EquipmentSlot(MainWindow.INSTANCE.SlotHandler.GetSlotFromName("Belt"), null);
        public EquipmentSlot Ring { get; set; } = new EquipmentSlot(MainWindow.INSTANCE.SlotHandler.GetSlotFromName("Ring"), null);
        public EquipmentSlot Potion { get; set; } = new EquipmentSlot(MainWindow.INSTANCE.SlotHandler.GetSlotFromName("Potion"), null);
        public EquipmentSlot Boots { get; set; } = new EquipmentSlot(MainWindow.INSTANCE.SlotHandler.GetSlotFromName("Boots"), null);

        public Equipment(bool mercenary = false)
        {
            if (mercenary)
                this.SetToMercenary();
        }
        public void SetToMercenary()
        {
            foreach (EquipmentSlot slot in this.GetEquipment())
            {
                slot.Mercenary = true;
                slot.Invisible = !Util.Util.MercenaryComp(slot.Slot.ID);
            }
        }
        public void Clear()
        {
            foreach (EquipmentSlot slot in this.GetEquipment())
                slot.SetItem(null);
        }

        public EquipmentSlot GetEquipmentSlotFromSlot(Slot slot)
        {
            foreach (EquipmentSlot equipment in this.GetEquipmentList())
            {
                if (equipment.Slot == slot)
                    return equipment;
            }
            return null;
        }

        public Item GetItemFromSlot(Slot slot)
        {
            List<EquipmentSlot> items = this.GetEquipmentList();
            foreach (EquipmentSlot equipment in items)
                if (equipment.Slot == slot)
                    return equipment.Item;
            return null;
        }

        public List<EquipmentSlot> GetEquipment()
        {
            List<EquipmentSlot> list = new List<EquipmentSlot>();
            list.Add(this.Amulet);
            list.Add(this.Helmet);
            list.Add(this.Charm);
            list.Add(this.Weapon);
            list.Add(this.Armor);
            list.Add(this.Shield);
            list.Add(this.Gloves);
            list.Add(this.Belt);
            list.Add(this.Ring);
            list.Add(this.Potion);
            list.Add(this.Boots);
            return list;
        }

        public List<EquipmentSlot> GetEquipmentList()
        {
            List<EquipmentSlot> list = new List<EquipmentSlot>();
            if (this.Amulet.Item != null) list.Add(this.Amulet);
            if (this.Helmet.Item != null) list.Add(this.Helmet);
            if (this.Charm.Item != null) list.Add(this.Charm);
            if (this.Weapon.Item != null) list.Add(this.Weapon);
            if (this.Armor.Item != null) list.Add(this.Armor);
            if (this.Shield.Item != null) list.Add(this.Shield);
            if (this.Gloves.Item != null) list.Add(this.Gloves);
            if (this.Belt.Item != null) list.Add(this.Belt);
            if (this.Ring.Item != null) list.Add(this.Ring);
            if (this.Potion.Item != null) list.Add(this.Potion);
            if (this.Boots.Item != null) list.Add(this.Boots);
            return list;
        }

        public List<Item> GetItems()
        {
            List<Item> list = new List<Item>();
            foreach (EquipmentSlot eq in this.GetEquipmentList())
                list.Add(eq.Item);
            return list;
        }

        public void RemoveItem(Item item)
        {
            if (this.Amulet.Item == item) Amulet = null;
            else if (this.Helmet.Item == item) Helmet = null;
            else if (this.Charm.Item == item) Charm = null;
            else if (this.Weapon.Item == item) Weapon = null;
            else if (this.Armor.Item == item) Armor = null;
            else if (this.Shield.Item == item) Shield = null;
            else if (this.Gloves.Item == item) Gloves = null;
            else if (this.Belt.Item == item) Belt = null;
            else if (this.Ring.Item == item) Ring = null;
            else if (this.Potion.Item == item) Potion = null;
            else if (this.Boots.Item == item) Boots = null;
        }

        public EquipmentSlot GetEquipmentSlot(int? id)
        {
            switch (id)
            {
                case 0:
                    return this.Helmet;
                    break;
                case 1:
                    return this.Armor;
                    break;
                case 2:
                    return this.Boots;
                    break;
                case 3:
                    return this.Weapon;
                    break;
                case 4:
                    return this.Gloves;
                    break;
                case 5:
                    return this.Amulet;
                    break;
                case 6:
                    return this.Charm;
                    break;
                case 7:
                    return this.Shield;
                    break;
                case 8:
                    return this.Ring;
                    break;
                case 9:
                    return this.Belt;
                    break;
                case 10:
                    return this.Potion;
                    break;
            }
            return null;
        }

        public void EquipItem(Item item, int? slot = null, bool refresh = true)
        {
            if (slot == null) slot = item.Slot.ID;

            this.GetEquipmentSlot(slot).SetItem(item);
            if (refresh)
                MainWindow.INSTANCE.RefreshListboxes();
        }

    }
}
