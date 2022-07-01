using HSeditor.Classes;
using HSeditor.Classes.Filter.Item;
using HSeditor.Classes.Item_Stats;
using HSeditor.Classes.Items;
using HSeditor.Classes.Other;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace HSeditor
{
    public class Item
    {
        public string? Name { get; set; }
        public string? DisplayName { get; set; }
        public int ID { get; set; }
        public int ItemLevel { get; set; }
        public Rarity? Rarity { get; set; }
        public Set? Set { get; set; }
        public int Quality { get; set; }
        public string QualityFormatted { get; set; }
        public int UpgradeLevel { get; set; }
        public string UpgradeLevelFormatted { get; set; }
        public Slot Slot { get; set; }
        public WeaponType WeaponType { get; set; }
        public int SocketCount { get; set; }
        public int RollID { get; set; }
        public Ability Ability { get; set; }
        public int AbilityLevel { get; set; }
        public Sockets Sockets { get; set; }
        public Stats Stats { get; set; }
        public string Sprite { get; set; }
        public string? AbilityString { get; set; }
        public bool Chase { get; private set; }
        public bool Favorite { get; set; }
        public bool Runeword { get; set; }
        public bool Stash { get; set; }

        // Reading from SaveFile
        public Item(int ItemLevel, int Quality, int Rarity, int ID, int UpgradeLevel, WeaponType WeaponType, Slot Slot, int SocketCount, Ability Ability, int RollID, int AbilityLevel, bool Chase, List<Rune> Runes)
        {
            this.Stash = false;
            this.ItemLevel = ItemLevel;
            this.ID = ID;
            this.Quality = Quality;
            this.QualityFormatted = Quality + "%";
            this.UpgradeLevel = UpgradeLevel;
            this.UpgradeLevelFormatted = UpgradeLevel + "/10";
            this.Slot = Slot;
            this.WeaponType = WeaponType;
            this.SocketCount = SocketCount;
            this.RollID = RollID;
            this.Ability = Ability == null ? MainWindow.INSTANCE.AbilityHandler.None : Ability;
            this.AbilityLevel = AbilityLevel;
            this.AbilityString = Ability == null ? null : Ability.Name + " [Level " + AbilityLevel + "]";
            this.Sockets = new Sockets(Runes);
            this.Chase = Chase;
            this.Rarity = MainWindow.INSTANCE.RarityHandler.GetRarityFromID(Rarity);
            Item? equivalent = Rarity == 1
                ? MainWindow.INSTANCE.ItemHandler.CheckForRuneword(this) == null ? MainWindow.INSTANCE.ItemHandler.GetEquivalent(this) : MainWindow.INSTANCE.ItemHandler.CheckForRuneword(this).DeepCopy()
                : MainWindow.INSTANCE.ItemHandler.GetEquivalent(this, Chase) == null ? null : MainWindow.INSTANCE.ItemHandler.GetEquivalent(this, Chase).DeepCopy();

            // Unknown item found
            if (equivalent == null)
            {
                this.Name = "Unknown Item";
                this.DisplayName = this.Name;
                this.Rarity = MainWindow.INSTANCE.RarityHandler.GetRarityFromID(Rarity);
                this.Set = MainWindow.INSTANCE.SetHandler.None;
                this.Stats = null;
                this.Sprite = @"pack://application:,,,/HSeditor;component/Resources/Placeholder.png";
                return;
            }

            this.Name = equivalent.Name;
            this.Rarity = equivalent.Rarity == null ? this.Rarity : equivalent.Rarity;
            this.Set = equivalent.Set == null ? MainWindow.INSTANCE.SetHandler.None : equivalent.Set;
            this.Sprite = equivalent.Sprite;
            this.Stats = equivalent.Stats == null ? null : equivalent.Stats.DeepCopy();
            this.Runeword = equivalent.Runeword;
            if (this.Name.Contains("Veil of M"))
                Console.WriteLine();
            if (this.Stats != null)
                this.Stats.Calculate(this);
        }

        // Reading from Database
        public Item(string Name, int ID, Rarity Rarity, Slot Slot, WeaponType WeaponType, Set Set, bool Chase = false, List<Rune> Runes = null)
        {
            this.Stash = false;
            this.Favorite = false;
            this.Name = Name;
            this.DisplayName = this.Name;
            this.ID = ID;
            this.ItemLevel = 151;
            this.Chase = Chase;
            this.Rarity = this.Chase && Rarity.Name == "Set" ? MainWindow.INSTANCE.RarityHandler.GetRarityFromEditorID(11) : Rarity;
            this.Slot = Slot;
            this.WeaponType = WeaponType;
            this.Sockets = new Sockets(Runes);
            this.Set = Set;
            this.SocketCount = Runes != null ? Runes.Count : 6;
            this.Sprite = Slot.ID != 3 ? File.Exists(Environment.CurrentDirectory + @$"\Sprites\Items\{this.Slot.Name}\{this.Name}.png") ? Environment.CurrentDirectory + @$"\Sprites\Items\{this.Slot.Name}\{this.Name}.png" : @"pack://application:,,,/HSeditor;component/Resources/Placeholder.png"
                : File.Exists(Environment.CurrentDirectory + @$"\Sprites\Items\{this.WeaponType.Name}\{this.Name}.png") ? Environment.CurrentDirectory + @$"\Sprites\Items\{this.WeaponType.Name}\{this.Name}.png" : @"pack://application:,,,/HSeditor;component/Resources/Placeholder.png";


            this.Quality = MainWindow.INSTANCE.ConfigHandler.Config.NewItem.Quality;
            this.QualityFormatted = Quality + "%";
            this.UpgradeLevel = MainWindow.INSTANCE.ConfigHandler.Config.NewItem.UpgradeLevel;
            this.UpgradeLevelFormatted = UpgradeLevel + "/10";
            this.RollID = MainWindow.INSTANCE.ConfigHandler.Config.NewItem.RollID;
            this.Ability = MainWindow.INSTANCE.ConfigHandler.Config.NewItem.Ability;
            this.AbilityLevel = MainWindow.INSTANCE.ConfigHandler.Config.NewItem.AbilityLevel;
            this.AbilityString = $"[{Ability.Name}]";
            if (Runes == null && Rarity.Name != "Runeword")
                this.Sockets = new Sockets(MainWindow.INSTANCE.ConfigHandler.Config.NewItem.Sockets.GetRuneList());
            this.SocketCount = this.Sockets.GetRuneList().Count;
            this.Runeword = this.Rarity.EditorID == 9;
        }

        // Generic
        public Item(string Name, int ID, Slot Slot, WeaponType WeaponType)
        {
            this.Name = Name;
            this.Runeword = false;
            this.DisplayName = this.Name;
            this.ID = ID;
            this.Slot = Slot;
            this.WeaponType = WeaponType;
            this.Set = MainWindow.INSTANCE.SetHandler.None;
            this.Quality = MainWindow.INSTANCE.ConfigHandler.Config.NewItem.Quality;
            this.QualityFormatted = Quality + "%";
            this.UpgradeLevel = MainWindow.INSTANCE.ConfigHandler.Config.NewItem.UpgradeLevel;
            this.UpgradeLevelFormatted = UpgradeLevel + "/10";
            this.RollID = MainWindow.INSTANCE.ConfigHandler.Config.NewItem.RollID;
            this.Ability = MainWindow.INSTANCE.ConfigHandler.Config.NewItem.Ability;
            this.AbilityLevel = MainWindow.INSTANCE.ConfigHandler.Config.NewItem.AbilityLevel;
            this.AbilityString = $"[{Ability.Name}]";
            this.Sockets = new Sockets(MainWindow.INSTANCE.ConfigHandler.Config.NewItem.Sockets.GetRuneList());
            this.SocketCount = this.Sockets.GetRuneList().Count;
            this.Sprite = Slot.ID != 3 ? File.Exists(Environment.CurrentDirectory + @$"\Sprites\Items\Generics\{this.Slot.Name}\{this.Name}.png") ? Environment.CurrentDirectory + @$"\Sprites\Items\Generics\{this.Slot.Name}\{this.Name}.png" : @"pack://application:,,,/HSeditor;component/Resources/Placeholder.png"
                : File.Exists(Environment.CurrentDirectory + @$"\Sprites\Items\Generics\{this.WeaponType.Name}\{this.Name}.png") ? Environment.CurrentDirectory + @$"\Sprites\Items\Generics\{this.WeaponType.Name}\{this.Name}.png" : @"pack://application:,,,/HSeditor;component/Resources/Placeholder.png";
        }

        // Config (New Item)
        public Item(int ItemLevel, int Quality, int UpgradeLevel, int RollID, Ability Ability, int AbilityLevel, List<Rune> Runes)
        {
            this.ItemLevel = ItemLevel;
            this.Quality = Quality;
            this.UpgradeLevel = UpgradeLevel;
            this.RollID = RollID;
            this.Ability = Ability;
            this.AbilityLevel = AbilityLevel;
            this.Sockets = new Sockets(Runes);
        }

        // Item Filter
        public Item(string Name, Slot Slot, WeaponType WeaponType)
        {
            this.Name = Name;
            this.Slot = Slot;
            this.WeaponType = WeaponType;
            this.Stats = new Stats();
        }

        public void Forge(int itemlevel, int quality, int upgradelevel, int roll, Ability ability, int abilitylevel, List<Rune> runes)
        {
            this.ItemLevel = itemlevel;
            this.Quality = quality;
            this.QualityFormatted = quality + "%";
            int maxupgrade = this.Rarity == null ? 10 : this.Rarity.MaxUpgradeLevel;
            this.UpgradeLevel = upgradelevel > maxupgrade ? maxupgrade : upgradelevel;
            this.UpgradeLevelFormatted = upgradelevel + "/10";
            this.RollID = roll;
            this.Ability = ability;
            this.AbilityLevel = abilitylevel;
            this.AbilityString = Ability.Name + " [Level " + AbilityLevel + "]";
            if (this.Stats != null)
                this.Stats.Calculate(this);

            if (this.Rarity != null && this.Rarity.Name == "Runeword") return;
            List<Rune> temp = new List<Rune>(runes);
            if (this.Stats != null)
            {
                temp = new List<Rune>();
                for (int i = 0; i < this.Stats.SocketsMax; i++)
                    temp.Add(runes[i]);
            }

            if (this.Rarity == null || this.Rarity.Name != "Runeword")
                this.Sockets = new Sockets(temp);
        }
        public string GetItemString()
        {
            string runes = "s";
            foreach (Rune rune in this.Sockets.GetRuneList())
            {
                if (rune.Name != "None")
                    runes += rune.IngameID + "|";
            }
            runes = runes == "s" ? "" : runes;
            this.Ability = this.Ability == null ? MainWindow.INSTANCE.AbilityHandler.GetAbilityFromID(0) : this.Ability;

            return new string($"\"{this.ItemLevel}|{this.Quality}|{this.Rarity.IngameID}|{this.ID}|{this.UpgradeLevel}|{this.WeaponType.ID}|{this.Slot.ID}|{this.SocketCount}|{this.Ability.ID}|{this.RollID}|0|0|0|0|{this.AbilityLevel}|0|0|{Convert.ToInt32(this.Chase)}|{runes}\"");
        }



        public Item DeepCopy()
        {
            Item temp = (Item)this.MemberwiseClone();
            temp.Stats = this.Stats == null ? null : this.Stats.DeepCopy();
            temp.Sockets = new Sockets(this.Sockets.GetRuneList());
            return temp;
        }
    }

    public class ItemHandler
    {
        public List<Item> AllItems { get; private set; }
        public List<Item> Generics { get; private set; }
        public List<Runeword> Runewords { get; private set; }
        public ItemFilter Filter { get; set; }

        public ItemHandler()
        {
            this.AllItems = this.GetAllItems().OrderBy(o => o.Slot.ID).ThenBy(o => o.WeaponType.ID).ThenBy(o => o.ID).ThenBy(o => o.Rarity.EditorID).ToList();
            this.Generics = this.GetGenerics();
            this.Runewords = this.GetAllRunewords();
        }

        public Stats GetDefaultStats(Item Item)
        {
            foreach (Item item in MainWindow.INSTANCE.StatHandler.DefaulStats)
                if (item.Name == Item.Name)
                    return item.Stats.DeepCopy();
            return null;
        }

        private Item GetRunewordItem(string name)
        {
            foreach (Item item in this.AllItems)
                if (item.Name == name)
                    return item;

            return null;
        }

        private List<Runeword> GetAllRunewords()
        {
            List<Runeword> runewords = new List<Runeword>();
            var result = MainWindow.INSTANCE.iniDB.Read("SELECT * FROM Runewords");

            while (result.Read())
            {
                List<string> bases = result.GetString("baseids").Split('|').ToList();
                List<int> baseids = new List<int>();
                bases.ForEach(o => { baseids.Add(Int32.Parse(o)); });

                Runeword rw = new Runeword(result.GetInt32("id"), result.GetString("name"), result.GetInt32("itemlevel"), MainWindow.INSTANCE.RuneHandler.GetRunesFromSaveString(result.GetString("runeids")), baseids, result.GetInt32("sockets"), this.GetRunewordItem(result.GetString("name")));
                runewords.Add(rw);
            }
            return runewords;
        }

        public List<Item> GetFilteredList()
        {
            List<Item> filteredList = new List<Item>();
            List<Item> joined = new List<Item>(this.AllItems);
            if (MainWindow.INSTANCE.ConfigHandler.Favorites != null)
                joined.AddRange(MainWindow.INSTANCE.ConfigHandler.Favorites);
            foreach (Item item in joined)
            {
                if (item.Rarity.EditorID == 11 && !this.Filter.ContainsRarity(MainWindow.INSTANCE.RarityHandler.GetRarityFromEditorID(11)))
                    continue;

                if (!item.Name.ToLower().Contains(this.Filter.Name.ToLower()) && this.Filter.Name != "Search..." && this.Filter.Name != "")
                    continue;
                if (item.WeaponType.ID != this.Filter.WeaponType.ID && this.Filter.WeaponType.ID != -1)
                    continue;
                if (item.Slot.ID != this.Filter.Slot.ID && this.Filter.Slot.ID != -1)
                    continue;

                if (Filter.GetFilteredStats().Count == 0 && Filter.GetFilteredRarities().Count == 0 && Filter.GetFilteredTiers().Count == 0)
                {
                    filteredList.Add(item);
                    continue;
                }

                if (!Filter.ContainsRarity(item.Rarity) && Filter.GetFilteredRarities().Count != 0)
                    continue;


                if (item.Stats == null || !Filter.ContainsTier(item.Stats.Tier))
                    if (Filter.GetFilteredTiers().Count != 0)
                        continue;
                if (item.Stats == null && Filter.GetFilteredStats().Count != 0)
                    continue;

                bool x = false;
                foreach (Stat Stat in Filter.GetFilteredStats())
                    if (!item.Stats.Contains(Stat) && Filter.GetFilteredStats().Count != 0)
                    {
                        if (Stat.Name == "Talent")
                        {
                            bool y = false;
                            foreach (Stat stat in item.Stats.StatList)
                                if (stat.DebugName.StartsWith("INV_TALENT_"))
                                {
                                    y = true;
                                    break;
                                }
                            if (y) continue;
                        }
                        if (Stat.Name.Contains("Damage"))
                        {
                            if (item.Stats.Contains(new DamageType(Stat.Name.Replace(" Damage", String.Empty), "test", "test")))
                                continue;
                        }
                        x = true;
                        break;
                    };
                if (x) continue;

                filteredList.Add(item);
            }

            foreach (Item item in this.Generics)
            {
                foreach (Rarity rarity in Filter.GetFilteredRarities())
                {
                    if (rarity.EditorID > 5) continue;
                    if (item.WeaponType.ID != this.Filter.WeaponType.ID && this.Filter.WeaponType.ID != -1)
                        continue;
                    if (item.Slot.ID != this.Filter.Slot.ID && this.Filter.Slot.ID != -1)
                        continue;
                    Item item2 = item.DeepCopy();
                    item2.Rarity = rarity;
                    item2.UpgradeLevel = MainWindow.INSTANCE.ConfigHandler.Config.NewItem.UpgradeLevel > item2.Rarity.MaxUpgradeLevel ? item2.Rarity.MaxUpgradeLevel : MainWindow.INSTANCE.ConfigHandler.Config.NewItem.UpgradeLevel;
                    filteredList.Add(item2);
                }
            }

            //if (MainWindow.INSTANCE.ConfigHandler.Favorites == null) return filteredList;
            //foreach (Item favorite in MainWindow.INSTANCE.ConfigHandler.Favorites)
            //    if (favorite.Slot == this.Filter.Slot && (this.Filter.WeaponType.ID == -1 || favorite.WeaponType == this.Filter.WeaponType))
            //        filteredList.Add(favorite);

            return filteredList;
        }

        private List<Item> GetAllItems()
        {
            List<Item> items = new List<Item>();
            var result = MainWindow.INSTANCE.iniDB.Read("SELECT * FROM Items");
            Item item;

            while (result.Read())
            {
                item = new Item(result.GetString("name"), result.GetInt32("itemid"), MainWindow.INSTANCE.RarityHandler.GetRarityFromEditorID(result.GetInt32("rarityid")), MainWindow.INSTANCE.SlotHandler.GetSlotFromID(result.GetInt32("slotid")), MainWindow.INSTANCE.WeaponTypeHandler.GetWeaponTypeFromID(result.GetInt32("weapontypeid")), MainWindow.INSTANCE.SetHandler.GetSetFromID(result.GetInt32("setid")), MainWindow.INSTANCE.RarityHandler.GetRarityFromEditorID(result.GetInt32("rarityid")).Name == "Heroic");
                items.Add(item);
                if (item.Rarity.Name == "Set")
                    items.Add(new Item(item.Name, item.ID, item.Rarity, item.Slot, item.WeaponType, item.Set, true));
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

        public Item GetEquivalent(Item item, bool chase = false)
        {
            Item? fallback = null;
            if (item.Rarity.IngameID >= 6)
            {
                foreach (Item item2 in AllItems)
                {
                    bool wtype = item.Slot.ID != 3 || item.WeaponType.ID == item2.WeaponType.ID;
                    if (item.ID == item2.ID && item.Slot.ID == item2.Slot.ID && wtype)
                    {
                        fallback = item2;
                        if (!chase || (chase && item2.Rarity.Name == "Heroic Set"))
                            return item2;
                    }
                }
                return fallback;
            }
            else
            {
                foreach (Item item2 in Generics)
                {
                    bool wtype = item.Slot.ID != 3 || item.WeaponType.ID == item2.WeaponType.ID;
                    if (item.ID == item2.ID && item.Slot.ID == item2.Slot.ID && wtype)
                        return item2;
                }
                return null;
            }
        }

        public Item GetItem(string Name)
        {
            foreach (Item item in AllItems)
                if (item.Name == Name || item.Name.Replace("'", "") == Name)
                    return item;

            return null;
        }

        public Item GetItem(int id, int slotid, int weapontypeid = 0)
        {
            foreach (Item item in AllItems)
                if (item.Slot.ID == slotid && item.WeaponType.ID == weapontypeid && item.ID == id)
                    return item;

            return null;
        }



        public Item ParseSaveString(string str)
        {
            try
            {
                string[] temp = str.Replace("'", String.Empty).Replace("\"", String.Empty).Split('|');
                int length = temp.Length;
                foreach (string s in temp)
                    if (s == "" || s == "\"") length--;
                int runeindex = -1;
                for (int i = 0; i < temp.Length; i++)
                {
                    if (temp[i].Contains('s'))
                    {
                        runeindex = i;
                        break;
                    }
                }
                bool chase = temp[17] == "1";
                List<Rune> runes = new List<Rune>();
                if (runeindex != -1)
                {
                    for (int i = runeindex; i <= runeindex + 5; i++)
                    {
                        if (i >= length)
                        {
                            runes.Add(MainWindow.INSTANCE.RuneHandler.Runes[0]);
                            continue;
                        }
                        if (temp[i] != "")
                            runes.Add(MainWindow.INSTANCE.RuneHandler.GetRuneFromID(Int32.Parse(temp[i].Trim('s'))));
                    }
                }
                else
                    for (int i = 0; i < 6; i++)
                        runes.Add(MainWindow.INSTANCE.RuneHandler.Runes[0]);

                return new Item(Int32.Parse(temp[0]), Int32.Parse(temp[1]), Int32.Parse(temp[2]), Int32.Parse(temp[3]), Int32.Parse(temp[4]), MainWindow.INSTANCE.WeaponTypeHandler.GetWeaponTypeFromID(Int32.Parse(temp[5])), MainWindow.INSTANCE.SlotHandler.GetSlotFromID(Int32.Parse(temp[6])), Int32.Parse(temp[7]), MainWindow.INSTANCE.AbilityHandler.GetAbilityFromID(Int32.Parse(temp[8])), Int32.Parse(temp[9]), Int32.Parse(temp[14]), chase, runes);
            }
            catch { return null; }
        }

        public Item CheckForRuneword(Item item)
        {
            foreach (Runeword runeword in this.Runewords)
            {
                if (runeword.Bases.Contains(item.ID) && runeword.SocketCount == item.SocketCount && compareRunes(item, runeword) && item.ItemLevel >= runeword.ItemLevel && item.Slot.ID == runeword.Item.Slot.ID)
                {
                    item.Name = runeword.Name;
                    item.Rarity = runeword.Item.Rarity;
                    item.Set = MainWindow.INSTANCE.SetHandler.Sets[0];
                    return runeword.Item;
                }
            }
            return null;
        }

        private bool compareRunes(Item Item, Runeword Runeword)
        {
            for (int i = 0; i < Item.Sockets.GetRuneList().Count; i++)
            {
                if (Item.Sockets.GetRuneList()[i].EditorID != Runeword.Sockets.GetRuneList()[i].EditorID) return false;
            }
            return true;
        }
    }
}
