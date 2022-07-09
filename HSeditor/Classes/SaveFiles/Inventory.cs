using HSeditor.Classes.Items;
using HSeditor.Classes.Other;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace HSeditor.SaveFiles
{
    public class Inventory
    {
        public Equipment Equipment { get; set; }
        public ObservableCollection<Item> InventoryItems { get; set; }
        public List<string> UnknownItems { get; set; }
        public ObservableCollection<Uber> UberItems { get; set; }
        public ObservableCollection<Rune> Runes { get; set; }

        public void AddItem(Item item, bool Refresh = true)
        {
            this.InventoryItems.Add(item);
            if (Refresh)
                MainWindow.INSTANCE.RefreshListboxes();
        }

        public List<Item> GetCompleteInventory()
        {
            List<Item> inventory = new List<Item>();
            foreach (Item item in this.Equipment.GetItems())
                inventory.Add(item);
            foreach (Item item in InventoryItems)
                inventory.Add(item);

            return inventory;
        }

        public ObservableCollection<Uber> GetFilteredUbers(string Text, UberType Type)
        {
            ObservableCollection<Uber> filteredUbers = new ObservableCollection<Uber>();
            foreach (Uber uber in this.UberItems)
            {
                if (!uber.Name.ToLower().Contains(Text.ToLower()) && Text != "Search..." && Text != "")
                    continue;
                if (uber.Type != Type && Type.Name != "All Types")
                    continue;

                filteredUbers.Add(uber);
            }
            return filteredUbers;
        }

        public ObservableCollection<Rune> GetFilteredRunes(string Text, RuneType Type)
        {
            ObservableCollection<Rune> filteredRunes = new ObservableCollection<Rune>();
            foreach (Rune rune in this.Runes)
            {
                if (!rune.Name.ToLower().Contains(Text.ToLower()) && !rune.Description.ToLower().Contains(Text.ToLower()) && Text != "Search..." && Text != "")
                    continue;
                if (rune.Type != Type && Type.Name != "All Types")
                    continue;
                filteredRunes.Add(rune);
            }
            return filteredRunes;
        }

        public void AddItem(Uber uberitem, int Amount)
        {
            Uber uber = uberitem.DeepCopy();
            foreach (Uber uber2 in this.UberItems)
            {
                if (uber2.ID == uber.ID)
                {
                    uber2.Amount += Amount;
                    return;
                }
            }
            uber.Amount = Amount;
            this.UberItems.Add(uber);
            MainWindow.INSTANCE.RefreshListboxes();
        }

        public void AddItem(Rune Rune, int Amount)
        {
            Rune rune = new Rune(Rune.IngameID, Rune.Amount);
            foreach (Rune rune2 in this.Runes)
            {
                if (rune2.EditorID == rune.EditorID)
                {
                    rune2.Amount += Amount;
                    return;
                }
            }
            rune.Amount = Amount;
            this.Runes.Add(rune);
            MainWindow.INSTANCE.RefreshListboxes();
        }

        public void RemoveItem(Item item)
        {
            this.InventoryItems.Remove(item);
            MainWindow.INSTANCE.labelInventoryCount.Content = MainWindow.INSTANCE.SaveFileHandler.SelectedFile.Inventory.InventoryItems.Count + "/100";
            MainWindow.INSTANCE.labelInventoryCountForge.Content = MainWindow.INSTANCE.SaveFileHandler.SelectedFile.Inventory.InventoryItems.Count + "/100";
        }

        public void RemoveItem(Uber uberitem)
        {
            this.UberItems.Remove(uberitem);
        }

        public void Clear()
        {
            this.Equipment.Clear();
            this.InventoryItems = new ObservableCollection<Item>();
            this.UberItems = new ObservableCollection<Uber>();
        }

        public Inventory(Equipment Equipment, ObservableCollection<Item> Inventory, ObservableCollection<Uber> Ubers, ObservableCollection<Rune> Runes)
        {
            this.Equipment = Equipment;
            this.InventoryItems = Inventory;
            this.UberItems = Ubers;
            if (this.UberItems.Count == 0)
                foreach (Uber uber in MainWindow.INSTANCE.UberHandler.Ubers)
                    this.UberItems.Add(uber.DeepCopy());
            this.Runes = Runes;
            if (this.Runes.Count == 0)
                foreach (Rune rune in MainWindow.INSTANCE.RuneHandler.RunesFiltered)
                    this.Runes.Add(rune.DeepCopy());

        }

        // Build File
        public Inventory(Equipment Equipment, ObservableCollection<Item> Inventory)
        {
            this.Equipment = Equipment;
            this.InventoryItems = Inventory;
        }
    }
}
