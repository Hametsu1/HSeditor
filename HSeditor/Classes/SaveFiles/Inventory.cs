using HSeditor.Classes.Items;
using HSeditor.Classes.Other;
using System.Collections.Generic;
using System.Linq;

namespace HSeditor.SaveFiles
{
    public class Inventory
    {
        public Equipment Equipment { get; set; }
        public List<Item> InventoryItems { get; set; }
        public List<Item> StashItems { get; set; }
        public List<string> UnknownItems { get; set; }

        public void AddItem(Item item, bool findPlace = true)
        {
            if (!MainWindow.INSTANCE.InventoryBoxHandler.isValidItem(item, MainWindow.INSTANCE.SelectedInv)) return;
            item = item.DeepCopy();
            item.Inv = MainWindow.INSTANCE.SelectedInv;
            if (findPlace || item.InvPos == null)
            {
                InventoryBox box = MainWindow.INSTANCE.InventoryBoxHandler.FindSpace(item, MainWindow.INSTANCE.InventoryBoxHandler.InventoryBoxes);
                if (box == null) return;
                item.InvPos = box.Position;
            }
            this.InventoryItems.Add(item);
        }

        public bool isSetPartActive(Item item)
        {
            bool active = false;
            if (item.Slot.Name == "Charm")
            {
                int y = (int)MainWindow.INSTANCE.InventoryBoxHandler.InventorySize.X - MainWindow.INSTANCE.InventoryBoxHandler.CharmWidth;
                active = this.InventoryItems.Find(o => o.Rarity.EditorID == 8 && o.Name == item.Name && o.Inv == ItemHandler.InvType.Main && o.InvPos.Value.X >= y) != null;
            }
            else
            {
                active = this.Equipment.isEquipped(item);
            }
            return active;
        }

        public List<Item> GetCompleteInventory()
        {
            return this.Equipment.GetItems().Concat(this.InventoryItems).ToList();
        }

        public void RemoveItem(Item item)
        {
            this.InventoryItems.Remove(item);
        }

        public void Clear()
        {
            this.Equipment.Clear();
            this.InventoryItems = new List<Item>();
        }

        public Inventory(Equipment Equipment, List<Item> Inventory, List<Item> StashItems)
        {
            this.Equipment = Equipment;
            this.InventoryItems = Inventory;
            this.StashItems = StashItems;
        }
    }
}
