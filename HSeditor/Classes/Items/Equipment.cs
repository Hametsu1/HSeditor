using HSeditor.Classes.Other;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace HSeditor.Classes.Items
{
    public class Equipment
    {
        List<EquipmentSlot> EquipmentSlots { get; set; }


        public Equipment(bool mercenary = false)
        {
            EquipmentSlots = MainWindow.INSTANCE.EquipmentSlotHandler.GetEquipmentSlots();

            if (mercenary)
                this.SetToMercenary();
        }
        public void SetToMercenary()
        {
            foreach (EquipmentSlot slot in this.GetEquipment())
            {
                slot.Mercenary = true;
            }
        }
        public void Clear()
        {
            foreach (EquipmentSlot slot in this.GetEquipment())
                slot.SetItem(null);
        }

        public EquipmentSlot GetEquipmentSlot(int id)
        {
            return this.EquipmentSlots.Find(o => o.ID == id);
        }

        /*
        public Item GetItemFromSlot(Slot slot)
        {
            List<EquipmentSlot> items = this.GetEquipmentList();
            foreach (EquipmentSlot equipment in items)
                if (equipment.Slot == slot)
                    return equipment.Item;
            return null;
        }*/

        public List<EquipmentSlot> GetEquipment()
        {
            return this.EquipmentSlots;
        }

        public List<EquipmentSlot> GetEquipmentList()
        {
            return this.EquipmentSlots.FindAll(o => o.Item != null);
        }

        public List<Item> GetItems()
        {
            return this.EquipmentSlots.Select(o => o.Item).ToList().FindAll(o => o != null);
        }

        public bool isEquipped(Item item)
        {
            return this.EquipmentSlots.Find(o => o.Item != null && o.Item.Name == item.Name) != null;
        }

        /*
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
            else if (this.Ring2.Item == item) Ring2 = null;
            else if (this.Boots.Item == item) Boots = null;
        }*/

        public EquipmentSlot GetEquipmentSlotFromItem(Item item)
        {
            List<EquipmentSlot> slots = this.EquipmentSlots.FindAll(o => o.Slot.ID == item.Slot.ID);
            if (slots.Count == 0) return null;
            return slots.Find(o => o.Item == null) != null ? slots.Find(o => o.Item == null) : slots[0];
        }

        public void EquipItem(Item item, int? slot = null)
        {
            if (slot == null)
            {
                EquipmentSlot eq = this.GetEquipmentSlotFromItem(item);
                if (eq == null) return;
                slot = eq.ID;
            }
            EquipmentSlot equipment = this.EquipmentSlots.Find(o => o.ID == slot);
            if (equipment == null) return;
            equipment.SetItem(item.DeepCopy());
        }

        public void EquipItemBySlotId(Item item, int slot)
        {
            EquipmentSlot equipment = this.EquipmentSlots.Find(o => o.Slot.ID == slot);
            if (equipment == null) return;
            equipment.SetItem(item);
        }

    }

    public class EquipmentSlotHandler
    {
        public List<EquipmentSlot> Slots { get; set; }

        public EquipmentSlotHandler()
        {
            Slots = this.GetSlots();
        }

        public List<EquipmentSlot> GetEquipmentSlots()
        {
            List<EquipmentSlot> slots = new List<EquipmentSlot>();
            foreach (EquipmentSlot slot in this.Slots)
                slots.Add(new EquipmentSlot(slot.ID, MainWindow.INSTANCE.SlotHandler.GetSlotFromID(slot.Slot.ID), null));

            return slots;
        }

        private List<EquipmentSlot> GetSlots()
        {
            List<EquipmentSlot> slots = new List<EquipmentSlot>();

            var result = MainWindow.INSTANCE.iniDB.Read("SELECT * FROM EquipmentSlots");

            while (result.Read())
            {
                slots.Add(new EquipmentSlot(result.GetInt32("id"), MainWindow.INSTANCE.SlotHandler.GetSlotFromID(result.GetInt32("slotid")), null));
            }

            return slots;
        }
    }
}
