using HSeditor.Classes.Other;
using HSeditor.Classes.SaveFiles;
using IniParser;
using IniParser.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows;

namespace HSeditor
{
    public class Config
    {
        public Item NewItem { get; set; }
        public HeroInfo NewChar { get; set; }
        public bool ShowGenericRarities { get; set; }
        public Point Resolution { get; set; }

        public Config(Item NewItem, HeroInfo NewChar, bool ShowGenericRarities, Point Resolution)
        {
            this.NewItem = NewItem;
            this.NewChar = NewChar;
            this.ShowGenericRarities = ShowGenericRarities;
            this.Resolution = Resolution;
        }

    }

    public class ConfigHandler
    {
        public Config Config { get; private set; }
        public List<Item> Favorites { get; private set; }
        readonly string path = Environment.GetEnvironmentVariable("LocalAppData") + $@"\Hero_Siege\hseditor\";
        public ConfigHandler()
        {
            this.LoadConfig();
        }

        public void LoadFavorites()
        {
            this.Favorites = new List<Item>();
            string path = this.path + "favorites.ini";
            if (!File.Exists(path)) return;


            var parser = new FileIniDataParser();
            IniData data = parser.ReadFile(path);

            foreach (SectionData section in data.Sections)
            {
                foreach (KeyData key in section.Keys)
                {
                    Item item = MainWindow.INSTANCE.ItemHandler.ParseSaveString(key.Value);
                    item.Favorite = true;
                    item.DisplayName = section.SectionName;
                    this.Favorites.Add(item);
                }
            }
        }

        public void AddFavorite(Item Item)
        {
            AddFavorite mb = new AddFavorite(Item.Name);
            mb.ShowDialog();
            if (mb.Cancel) return;

            string name = mb.textBlockName.Text;
            Item = Item.DeepCopy();
            Item.Favorite = true;
            Item.DisplayName = name;
            this.Favorites.Add(Item);
            this.WriteFavorites();
            MainWindow.INSTANCE.UpdateItemFilter();
        }

        public void RemoveFavorite(Item Item)
        {
            this.Favorites.Remove(Item);
            this.WriteFavorites();
            MainWindow.INSTANCE.UpdateItemFilter();
        }

        private void WriteFavorites()
        {
            string path = this.path + "favorites.ini";
            if (File.Exists(path)) File.Delete(path);
            StreamWriter sr = new StreamWriter(path);
            sr.Close();
            var parser = new FileIniDataParser();
            IniData data = parser.ReadFile(path);


            foreach (Item item in this.Favorites)
            {
                data.Sections.AddSection(item.DisplayName);
                data[item.DisplayName].AddKey($"item", item.GetItemString());
            }

            IniData parsedINIDataToBeSaved = data;
            parser.WriteFile(path, parsedINIDataToBeSaved);
        }

        public List<Item> GetFavoritesInSlot(Slot slot, WeaponType weapon)
        {
            List<Item> items = new List<Item>();
            if (slot == null || weapon == null || this.Favorites == null) return items;
            foreach (Item item in Favorites)
                if (item.Slot.ID == slot.ID && (item.WeaponType == weapon || weapon.ID == -1))
                    items.Add(item);
            return items;
        }

        public void LoadConfig()
        {
            string path = this.path + "config.ini";
            if (!File.Exists(path)) { WriteConfig(); LoadConfig(); }
            try
            {
                var parser = new FileIniDataParser();
                IniData data = parser.ReadFile(path);

                string[] temp = data["NewItem"]["Runes"].Split('|');
                List<Rune> Runes = new List<Rune>();
                foreach (string s in temp)
                    Runes.Add(MainWindow.INSTANCE.RuneHandler.GetRuneFromID(Convert.ToInt32(s)));
                Item Item = new Item(Convert.ToInt32(data["NewItem"]["ItemLevel"]), Convert.ToInt32(data["NewItem"]["Quality"]), Convert.ToInt32(data["NewItem"]["UpgradeLevel"]), Convert.ToInt32(data["NewItem"]["RollID"]), MainWindow.INSTANCE.AbilityHandler.GetAbilityFromID(Convert.ToInt32(data["NewItem"]["Ability"])), Convert.ToInt32(data["NewItem"]["AbilityLevel"]), Runes);


                HeroInfo HeroInfo = new HeroInfo(MainWindow.INSTANCE.ClassHandler.GetClassFromID(Convert.ToInt32(data["NewChar"]["Class"])), data["NewChar"]["Name"], Convert.ToInt32(data["NewChar"]["Level"]), Convert.ToInt32(data["NewChar"]["HeroLevel"]), 0, 0, data["NewChar"]["Hardcore"] == "True", MainWindow.INSTANCE.TalentHandler.GetHeroTalentList(), MainWindow.INSTANCE.TalentHandler.GetEmptyActiveTalents());
                Point point = new Point(Convert.ToInt32(data["Misc"]["Resolution"].Split('x')[0]), Convert.ToInt32(data["Misc"]["Resolution"].Split('x')[1]));
                this.Config = new Config(Item, HeroInfo, data["ItemFilter"]["GenericRarities"] == "True", point);
            }
            catch
            {
                WriteConfig();
                LoadConfig();
            }
        }

        public void WriteConfig(bool setToDefaults = false)
        {
            string path = this.path + "config.ini";
            if (File.Exists(path)) File.Delete(path);
            StreamWriter sr = new StreamWriter(path);
            sr.Close();
            var parser = new FileIniDataParser();
            IniData data = parser.ReadFile(path);

            data.Sections.AddSection("NewItem");
            data["NewItem"].AddKey("Quality", this.Config == null || setToDefaults ? "100" : this.Config.NewItem.Quality.ToString());
            data["NewItem"].AddKey("UpgradeLevel", this.Config == null || setToDefaults ? "10" : this.Config.NewItem.UpgradeLevel.ToString());
            data["NewItem"].AddKey("ItemLevel", this.Config == null || setToDefaults ? "151" : this.Config.NewItem.ItemLevel.ToString());
            data["NewItem"].AddKey("RollID", this.Config == null || setToDefaults ? "-1" : this.Config.NewItem.RollID.ToString());
            data["NewItem"].AddKey("Ability", this.Config == null || setToDefaults ? "0" : this.Config.NewItem.Ability.ID.ToString());
            data["NewItem"].AddKey("AbilityLevel", this.Config == null || setToDefaults ? "0" : this.Config.NewItem.AbilityLevel.ToString());
            data["NewItem"].AddKey("Runes", this.Config == null || setToDefaults ? "0|0|0|0|0|0" : $"{this.Config.NewItem.Sockets.Rune1.IngameID}|{this.Config.NewItem.Sockets.Rune2.IngameID}|{this.Config.NewItem.Sockets.Rune3.IngameID}|{this.Config.NewItem.Sockets.Rune4.IngameID}|{this.Config.NewItem.Sockets.Rune5.IngameID}|{this.Config.NewItem.Sockets.Rune6.IngameID}");

            data.Sections.AddSection("NewChar");
            data["NewChar"].AddKey("Name", this.Config == null || setToDefaults ? "New Char" : this.Config.NewChar.Name);
            data["NewChar"].AddKey("Class", this.Config == null || setToDefaults ? "0" : this.Config.NewChar.Class.ID.ToString());
            data["NewChar"].AddKey("Level", this.Config == null || setToDefaults ? "100" : this.Config.NewChar.Level.ToString());
            data["NewChar"].AddKey("HeroLevel", this.Config == null || setToDefaults ? "0" : this.Config.NewChar.HeroLevel.ToString());
            data["NewChar"].AddKey("Hardcore", this.Config == null || setToDefaults ? "False" : this.Config.NewChar.Hardcore ? "True" : "False");

            data.Sections.AddSection("ItemFilter");
            data["ItemFilter"].AddKey("GenericRarities", this.Config == null || setToDefaults ? "False" : this.Config.ShowGenericRarities ? "True" : "False");
            data.Sections.AddSection("Misc");
            data["Misc"].AddKey("Resolution", this.Config == null || setToDefaults ? "1114x620" : $"{this.Config.Resolution.X}x{this.Config.Resolution.Y}");

            IniData parsedINIDataToBeSaved = data;
            parser.WriteFile(path, parsedINIDataToBeSaved);
        }
    }
}
