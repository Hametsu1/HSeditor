using HSeditor.Classes;
using HSeditor.Classes.Filter.Item;
using HSeditor.Classes.Items;
using HSeditor.Classes.Other;
using HSeditor.Classes.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace HSeditor
{
    public class Item
    {
        public string? Name { get; set; }
        public int ID { get; set; }
        public Rarity? Rarity { get; set; }
        public Set? Set { get; set; }
        public Slot Slot { get; set; }
        public WeaponType WeaponType { get; set; }
        public int RollID { get; set; }
        public Sockets Sockets { get; set; }
        public string Sprite { get; set; }
        public Point? InvPos { get; set; }
        public List<InventoryBox> Fields { get; set; }
        public ItemHandler.InvType? Inv { get; set; }
        public Border InvImage { get; set; }
        public Point Size { get; private set; }
        public string BindingProp1 { get; set; }
        public string BindingProp2 { get; set; }
        public string BindingProp3 { get; set; }
        public string BindingProp4 { get; set; }
        public Grid SizeGrid { get { return this.GetSizeGrid(); } }
        public JObject SaveItem { get; set; }
        public ItemDescription ItemDescription { get; set; }
        public bool isRuneword { get { return MainWindow.INSTANCE.ItemHandler.Runewords.Find(o => o.Name == this.Name) != null; } set { } }


        // Reading from SaveFile
        public Item(int Rarity, int ID, WeaponType WeaponType, Slot Slot, int RollID, List<Rune> Runes, int posX = 0, int posY = 0, ItemHandler.InvType Inv = ItemHandler.InvType.Main)
        {
            this.ID = ID;
            this.Fields = new List<InventoryBox>();
            this.Slot = Slot;
            this.WeaponType = WeaponType;
            this.RollID = RollID;
            this.Sockets = new Sockets(Runes);

            this.InvPos = new Point(posX, posY);
            this.Inv = Inv;

            this.Rarity = MainWindow.INSTANCE.RarityHandler.GetRarityFromID(Rarity);

            this.SetEquivalent(Rarity);
        }

        public void SetEquivalent(int rarity, Item equi = null)
        {
            Item? equivalent;
            if (equi != null) equivalent = equi;
            else if (rarity == 1 && MainWindow.INSTANCE.ItemHandler.CheckForRuneword(this) != null) equivalent = MainWindow.INSTANCE.ItemHandler.CheckForRuneword(this);
            else equivalent = MainWindow.INSTANCE.ItemHandler.GetEquivalent(this);


            // Unknown item found
            if (equivalent == null)
            {
                this.Name = "Unknown Item";
                this.Set = MainWindow.INSTANCE.SetHandler.None;
                this.SetSprite();
                this.Size = new Point(1, 1);
                return;
            }

            this.ID = equivalent.isRuneword ? this.ID : equivalent.ID;
            this.Name = equivalent.Name;
            this.Rarity = this.Rarity.IngameID == equivalent.Rarity.IngameID ? equivalent.Rarity : this.Rarity;
            this.Set = equivalent.Set == null ? MainWindow.INSTANCE.SetHandler.None : equivalent.Set;
            this.Sprite = equivalent.Sprite;
            this.Size = equivalent.Size;
            this.Slot = equivalent.Slot;
            this.ItemDescription = equivalent.ItemDescription;
        }

        // Reading from Database
        public Item(string Name, int ID, Rarity Rarity, Slot Slot, WeaponType WeaponType, Set Set, Point size, List<Rune> Runes = null)
        {
            this.Name = Name;
            this.ID = ID;
            this.Rarity = Rarity;
            this.Slot = Slot;
            this.WeaponType = WeaponType;
            this.Sockets = new Sockets(Runes);
            this.Set = Set;
            this.Size = size;
            this.SaveItem = this.GetItemObject();
            this.SetSprite();

            this.RollID = -1;
            if (Runes == null && Rarity.Name != "Runeword")
                this.Sockets = new Sockets(new List<Rune>());
        }

        // Generic
        public Item(string Name, int ID, Slot Slot, WeaponType WeaponType)
        {
            this.Name = Name;
            this.ID = ID;
            this.Slot = Slot;
            this.WeaponType = WeaponType;
            this.Set = MainWindow.INSTANCE.SetHandler.None;
            this.RollID = -1;
            this.Sockets = new Sockets(new List<Rune>());
            this.SetSprite();
        }

        private readonly string _generic = Environment.CurrentDirectory + @$"\Sprites\Items\Generics\";
        private readonly string _default = Environment.CurrentDirectory + @$"\Sprites\Items\";

        private void SetSprite()
        {
            string path = _default;
            if (this.Slot.ID == 3) path += $"{this.WeaponType.Name}\\";
            else path += $"{this.Slot.Name}";

            var files = ItemHandler.SpriteList.ContainsKey(path) ? ItemHandler.SpriteList[path] : null;
            if (files == null)
            {
                files = Directory.Exists(path) ? Directory.GetFiles(path).ToList() : new List<string>();
                ItemHandler.SpriteList.Add(path, files);
            }

            string filename = this.Name.Clean().ToLower();
            string spritePath = files.Find(o => Path.GetFileNameWithoutExtension(o).Clean().ToLower() == filename);
            this.Sprite = spritePath;
            if (this.Sprite == null)
            {
                if (Slot != null && Slot.Sprite != "" && Slot.Sprite != null) this.Sprite = this.Slot.ID == 3 ? this.WeaponType.Sprite : this.Slot.Sprite;
                else this.Sprite = @"pack://application:,,,/HSeditor;component/Resources/Placeholder.png";
            }
        }

        public Grid GetSizeGrid()
        {
            Grid grid = new Grid { HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Center };
            StackPanel stackPanel1 = new StackPanel { Orientation = Orientation.Horizontal, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center };
            grid.Children.Add(stackPanel1);
            for (int x = 0; x < Size.X; x++)
            {
                StackPanel stackPanel = new StackPanel { Orientation = Orientation.Vertical, Margin = new Thickness(0, 0, x == (Size.X - 1) ? 0 : 2, 0) };
                for (int y = 0; y < Size.Y; y++)
                {
                    Grid grid2 = new Grid { Height = 3.5, Width = 3.5, Margin = new Thickness(0, 0, 0, y == (Size.Y - 1) ? 0 : 2), Background = Util.ColorFromString(this.Rarity.Color), HorizontalAlignment = HorizontalAlignment.Center };
                    stackPanel.Children.Add(grid2);
                }
                stackPanel1.Children.Add(stackPanel);
            }
            return grid;
        }

        public string GetItemString()
        {
            return "\"" + Convert.ToBase64String(Encoding.UTF8.GetBytes(this.GetItemObject().ToString(Formatting.Indented))) + "\"";
        }

        public JObject GetItemObject()
        {
            JObject obj = this.SaveItem;
            if (obj == null)
            {
                using (StreamReader sr = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(Assembly.GetExecutingAssembly().GetManifestResourceNames().Single(str => str.EndsWith("DefaultItem.txt")))))
                {
                    obj = JObject.Parse(sr.ReadToEnd());
                }
            }
            obj["type"] = this.Slot.ID;
            obj["id"] = this.ID;
            obj["weapon_type"] = this.WeaponType.ID;
            List<Rune> runes = this.Sockets.GetRuneList();
            for (int i = 1; i <= 6; i++)
            {
                obj[$"socket_{i}"] = runes[i - 1].ID;
            }
            obj["seed"] = this.RollID;
            obj["rarity"] = this.Rarity.IngameID;
            obj["x"] = this.InvPos == null ? "0.0" : this.InvPos.Value.X;
            obj["y"] = this.InvPos == null ? "0.0" : this.InvPos.Value.Y;
            this.SaveItem = obj;
            return obj;
        }

        public void UpdateData()
        {
            JObject obj = this.SaveItem;


            Item equivalent = MainWindow.INSTANCE.ItemHandler.ParseJSONObject(obj);
            this.Name = equivalent.Name;
            this.Sprite = equivalent.Sprite;
            this.Size = equivalent.Size;
            this.ID = obj.ContainsKey("id") ? (int)obj["id"] : equivalent.ID;
            this.Set = equivalent.Set;
            this.Slot = equivalent.Slot;
            this.WeaponType = equivalent.WeaponType;
            this.ItemDescription = equivalent.ItemDescription;
            this.Rarity = equivalent.Rarity;
            List<Rune> runes = new List<Rune>();
            for (int i = 1; i <= 6; i++)
            {
                int id = obj.ContainsKey($"socket_{i}") ? (int)obj[$"socket_{i}"] : 0;
                runes.Add(MainWindow.INSTANCE.RuneHandler.GetRuneFromID(id));
            }
            this.Sockets = new Sockets(runes);
            this.RollID = obj.ContainsKey("seed") ? (int)obj["seed"] : -1;
        }

        public Item DeepCopy()
        {
            Item temp = (Item)this.MemberwiseClone();
            temp.Sockets = new Sockets(this.Sockets.GetRuneList());
            temp.SaveItem = JObject.Parse(this.SaveItem.ToString());
            return temp;
        }
    }

    public class ItemHandler
    {
        public List<Item> AllItems { get; private set; }
        public List<Item> Generics { get; private set; }
        public List<Item> Runewords { get; private set; }
        public ItemFilter Filter { get; set; }
        public static Dictionary<string, List<string>> SpriteList = new Dictionary<string, List<string>>();
        public readonly int StashIndex = 6;

        public enum InvType
        {
            Socketables = 0,
            Potion = 1,
            Main = 2,
            Extra1 = 3,
            Extra2 = 4,
            Extra3 = 5,
            Stash0 = 6,
            Stash1 = 7,
            Stash2 = 8,
            Stash3 = 9,
            Stash4 = 10,
            Stash5 = 11,
        }

        public ItemHandler()
        {
            this.Generics = this.GetGenerics();
            this.Runewords = this.GetAllRunewords();
            this.AllItems = this.GetAllItems().Concat(this.Runewords).OrderBy(o => o.Slot.ID).ThenBy(o => o.WeaponType.ID).ThenBy(o => o.ID).ThenBy(o => o.Rarity.EditorID).ToList();
        }

        private List<Item> GetAllRunewords()
        {
            List<Item> runewords = new List<Item>();
            var result = MainWindow.INSTANCE.iniDB.Read("SELECT * FROM RunewordSeeds");
            Dictionary<int, int> seeds = new Dictionary<int, int>();
            while (result.Read())
            {
                seeds.Add(result.GetInt32("runeamount"), result.GetInt32("seed"));
            }
            result.Close();

            result = MainWindow.INSTANCE.iniDB.Read("SELECT * FROM Runewords");
            while (result.Read())
            {
                Item item = new Item(
                    result.GetString("name"),
                    0,
                    MainWindow.INSTANCE.RarityHandler.GetRarityFromEditorID(9),
                    MainWindow.INSTANCE.SlotHandler.GetSlotFromID(result.GetInt32("slotid")),
                    MainWindow.INSTANCE.WeaponTypeHandler.GetWeaponTypeFromID(Convert.ToInt32(result.GetString("weapontypeid").Split('|')[0])),
                    MainWindow.INSTANCE.SetHandler.GetSetFromID(-1),
                    new Point(result.GetInt32("x"), result.GetInt32("y")));
                List<Rune> runes = new List<Rune>();
                if (result.GetString("name") == "Rainbow")
                    Console.WriteLine();
                result.GetString("runes").Split(',').ToList().ForEach(o => runes.Add(MainWindow.INSTANCE.RuneHandler.GetRuneFromName(o)));
                item.Sockets = new Sockets(runes);
                item.RollID = seeds[result.GetString("runes").Split(',').Length];
                runewords.Add(item);
            }
            result.Close();
            return runewords;
        }

        public List<Item> GetFilteredList()
        {
            List<Item> joined = new List<Item>(this.AllItems);
            if (MainWindow.INSTANCE.ConfigHandler.Favorites != null)
                joined.AddRange(MainWindow.INSTANCE.ConfigHandler.Favorites);

            List<Item> returns = new List<Item>();
            foreach (Item item in joined)
            {
                if (!item.Name.ToLower().Contains(this.Filter.Name.ToLower()) && this.Filter.Name != "Search..." && this.Filter.Name != "")
                    continue;

                if (item.Rarity.EditorID == 11 && !this.Filter.ContainsRarity(MainWindow.INSTANCE.RarityHandler.GetRarityFromEditorID(11)))
                    continue;


                if (item.WeaponType.ID != this.Filter.WeaponType.ID && this.Filter.WeaponType.ID != -1)
                    continue;
                if (item.Slot.ID != this.Filter.Slot.ID && this.Filter.Slot.ID != -1)
                    continue;

                if (!Filter.ContainsRarity(item.Rarity) && Filter.GetFilteredRarities().Count != 0)
                    continue;

                returns.Add(item);
            }

            return returns;
        }

        private List<Item> GetAllItems()
        {
            List<Item> items = new List<Item>();
            var result = MainWindow.INSTANCE.iniDB.Read("SELECT * FROM Items");
            Item item;

            while (result.Read())
            {
                item = new Item(
                    result.GetString("name"),
                    result.GetInt32("ingameid"),
                    MainWindow.INSTANCE.RarityHandler.GetRarityFromEditorID(result.GetInt32("rarityid")),
                    MainWindow.INSTANCE.SlotHandler.GetSlotFromEditorID(result.GetInt32("slotid")),
                    MainWindow.INSTANCE.WeaponTypeHandler.GetWeaponTypeFromID(result.GetInt32("weapontypeid")),
                    MainWindow.INSTANCE.SetHandler.GetSetFromID(result.GetInt32("setid")),
                    new Point(result.GetInt32("size_x"), result.GetInt32("size_y")));
                items.Add(item);
            }
            result.Close();

            return items;
        }

        private List<Item> GetGenerics()
        {
            List<Item> items = new List<Item>();
            var result = MainWindow.INSTANCE.iniDB.Read("SELECT * FROM Generics");

            while (result.Read())
            {
                items.Add(new Item(result.GetString("name"), result.GetInt32("id"), MainWindow.INSTANCE.SlotHandler.GetSlotFromID(result.GetInt32("slotid")), MainWindow.INSTANCE.WeaponTypeHandler.GetWeaponTypeFromID(result.GetInt32("weapontypeid"))));
            }
            result.Close();

            return items;
        }

        public Item GetEquivalent(Item item)
        {
            try
            {
                bool isgeneric = item.Slot.ID == 16 ? false : (item.Rarity.IngameID >= 1 && item.Rarity.IngameID <= 5);
                List<Item> list = isgeneric ? new List<Item>() : this.AllItems;
                return list.Find(o => !o.isRuneword && o.Slot.ID == item.Slot.ID && o.WeaponType.ID == item.WeaponType.ID && o.ID == item.ID);
            }
            catch
            {
                return null;
            }
        }

        public Item ParseSaveString(string str)
        {
            try
            {
                return str.Contains("|") ? null : ParseJSONObject(JObject.Parse(System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(str.Replace("'", String.Empty).Replace("\"", String.Empty)))));
            }
            catch { return null; }
        }

        public Item ParseJSONObject(JObject item)
        {
            int x = -1; int y = -1;
            if (item["x"] != null)
            {
                x = (int)Double.Parse(item["x"].ToString());
                y = (int)Double.Parse(item["y"].ToString());
            }

            List<Rune> runes = new List<Rune>();
            runes.Add(item["socket_1"] == null ? new Rune(0, "None") : MainWindow.INSTANCE.RuneHandler.GetRuneFromID((int)item["socket_1"]));
            runes.Add(item["socket_2"] == null ? new Rune(0, "None") : MainWindow.INSTANCE.RuneHandler.GetRuneFromID((int)item["socket_2"]));
            runes.Add(item["socket_3"] == null ? new Rune(0, "None") : MainWindow.INSTANCE.RuneHandler.GetRuneFromID((int)item["socket_3"]));
            runes.Add(item["socket_4"] == null ? new Rune(0, "None") : MainWindow.INSTANCE.RuneHandler.GetRuneFromID((int)item["socket_4"]));
            runes.Add(item["socket_5"] == null ? new Rune(0, "None") : MainWindow.INSTANCE.RuneHandler.GetRuneFromID((int)item["socket_5"]));
            runes.Add(item["socket_6"] == null ? new Rune(0, "None") : MainWindow.INSTANCE.RuneHandler.GetRuneFromID((int)item["socket_6"]));


            return new Item((int)item["rarity"], (int)item["id"], MainWindow.INSTANCE.WeaponTypeHandler.GetWeaponTypeFromID((int)item["weapon_type"]), MainWindow.INSTANCE.SlotHandler.GetSlotFromID((int)item["type"]), (int)item["seed"], runes, x, y);
        }

        public Item CheckForRuneword(Item item)
        {
            Item? runeword = this.Runewords.Find(rw =>
            {
                if (item.Slot.ID != rw.Slot.ID) return false;
                if (item.WeaponType.ID != rw.WeaponType.ID) return false;
                if (item.Sockets.Rune1.ID != rw.Sockets.Rune1.ID) return false;
                if (item.Sockets.Rune2.ID != rw.Sockets.Rune2.ID) return false;
                if (item.Sockets.Rune3.ID != rw.Sockets.Rune3.ID) return false;
                if (item.Sockets.Rune4.ID != rw.Sockets.Rune4.ID) return false;
                if (item.Sockets.Rune5.ID != rw.Sockets.Rune5.ID) return false;
                if (item.Sockets.Rune6.ID != rw.Sockets.Rune6.ID) return false;
                return true;
            });

            return runeword;
        }
    }
}
