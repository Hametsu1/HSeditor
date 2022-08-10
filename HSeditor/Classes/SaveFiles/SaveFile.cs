using HSeditor.Classes;
using HSeditor.Classes.Items;
using HSeditor.Classes.Merc;
using HSeditor.Classes.Other;
using HSeditor.Classes.SaveFiles;
using HSeditor.Classes.Util;
using HSeditor.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;

namespace HSeditor.SaveFiles
{
    public class SaveFile
    {
        public int ID { get; private set; }
        public string Path { get; private set; }
        public Inventory? Inventory { get; set; }
        public ObservableCollection<Relic>? Relics { get; set; }
        public HeroInfo HeroInfo { get; set; }
        public string Sprite { get; set; }
        public string Color { get; set; }
        public bool NewChar { get; private set; }
        public Mercenaries? Mercenaries { get; set; }

        public SaveFile(int ID, string Path, HeroInfo HeroInfo, Mercenaries Mercenaries, Inventory Inventory = null, ObservableCollection<Relic> Relics = null, bool newchar = false)
        {
            this.ID = ID;
            this.Path = Path;
            this.Mercenaries = Mercenaries == null ? new Mercenaries(new Mercenary("New Merc", MainWindow.INSTANCE.MercenaryHandler.GetMercenaryTypeFromName("Melee"), new Equipment(), true), new Mercenary("New Merc", MainWindow.INSTANCE.MercenaryHandler.GetMercenaryTypeFromName("Ranged"), new Equipment(), false), new List<MercenaryTalent>()) : Mercenaries;
            this.Inventory = Inventory == null ? new Inventory(new Equipment(), new ObservableCollection<Item>(), new ObservableCollection<Uber>(), new ObservableCollection<Rune>()) : Inventory;
            this.Relics = Relics == null ? new ObservableCollection<Relic>() : Relics;
            this.HeroInfo = HeroInfo;
            this.Sprite = newchar ? Environment.CurrentDirectory + @$"\Sprites\Classes\NewChar.png" : HeroInfo.Class.Sprite;
            this.Color = newchar ? "#99AAB5" : this.ID % 2 != 0 ? "#bababa" : "#F6F6F6";
            this.NewChar = newchar;
        }


        // Build File
        public SaveFile(HeroInfo HeroInfo, Inventory Inventory, Mercenaries Mercenaries, ObservableCollection<Relic> Relics)
        {
            this.HeroInfo = HeroInfo;
            this.Mercenaries = Mercenaries;
            this.Inventory = Inventory == null ? new Inventory(new Equipment(), new ObservableCollection<Item>(), new ObservableCollection<Uber>(), new ObservableCollection<Rune>()) : Inventory;
            this.Relics = Relics == null ? new ObservableCollection<Relic>() : Relics;
        }

        public void AddRelic(Relic relic)
        {
            if (this.Relics.Count >= 30) return;
            if (relic.Type.Name == "Active" && this.GetActiveRelic() != null) return;
            this.Relics.Add(relic);
            MainWindow.INSTANCE.UpdateRelicFilter();
        }

        public void RemoveRelic(Relic relic)
        {
            this.Relics.Remove(relic);
            MainWindow.INSTANCE.UpdateRelicFilter();
        }

        public Relic GetActiveRelic()
        {
            foreach (Relic relic in this.Relics)
                if (relic.Type.Name == "Active")
                    return relic;
            return null;
        }

        public void ImportBuild(Build build)
        {
            SaveFile save = build.SaveFile;
            if (save.Inventory.InventoryItems.Count > 0)
            {
                this.Inventory.InventoryItems = new ObservableCollection<Item>();
                foreach (Item item in save.Inventory.InventoryItems) this.Inventory.InventoryItems.Add(item.DeepCopy());
            }

            if (save.Inventory.Equipment.GetItems().Count > 0)
            {
                this.Inventory.Equipment.Clear();
                foreach (EquipmentSlot eq in save.Inventory.Equipment.GetEquipmentList())
                    this.Inventory.Equipment.EquipItem(eq.Item.DeepCopy(), eq.Slot.ID, false);
            }

            if (save.Relics.Count > 0)
                this.Relics = save.Relics;

            this.HeroInfo.Class = save.HeroInfo.Class.DeepCopy();
            this.HeroInfo.HeroTalents = new List<HeroTalent>(save.HeroInfo.HeroTalents);
            this.HeroInfo.ActiveTalents = new List<ActiveTalent>(save.HeroInfo.ActiveTalents);
            this.HeroInfo.Level = save.HeroInfo.Level;
            this.HeroInfo.HeroLevel = save.HeroInfo.HeroLevel;
            if (save.Mercenaries != null)
            {
                if (save.Mercenaries.GetSelected() == 0)
                    MainWindow.INSTANCE.buttonMeleeMerc.IsChecked = true;
                else
                    MainWindow.INSTANCE.buttonRangedMerc.IsChecked = true;
                if (save.Mercenaries.Talents.GetPointsSpent() != 0) this.Mercenaries.Talents = new MercenaryTalents(save.Mercenaries.Talents.AllTalents);
                this.Mercenaries.SetSelected(save.Mercenaries.GetSelected());
                if (save.Mercenaries.GetSelectedMerc().Equipment.GetItems().Count != 0)
                {
                    this.Mercenaries.GetSelectedMerc().Equipment.Clear();
                    foreach (EquipmentSlot slot in save.Mercenaries.GetSelectedMerc().Equipment.GetEquipmentList())
                        this.Mercenaries.GetSelectedMerc().Equipment.EquipItem(slot.Item.DeepCopy(), slot.Slot.ID, false);
                }
                MainWindow.INSTANCE.SaveFileHandler.Shop.SelectedMercenary = build.SaveFile.Mercenaries.GetSelected();
                MainWindow.INSTANCE.labelMercTalentPointsSpent.Text = $"{save.Mercenaries.Talents.GetPointsSpent()}/{save.HeroInfo.Level}";
            }


            MainWindow.INSTANCE.RefreshListboxes();
            MainWindow.INSTANCE.UpdateHeroInfo();
            MainWindow.INSTANCE.textBoxHeroName.Text = build.Name;
        }

        public void Save()
        {
            try
            {
                string path = Environment.GetEnvironmentVariable("LocalAppData") + $@"\Hero_Siege\save_folder\herosiege{this.ID}.pas";
                if (!File.Exists(path))
                {
                    using (StreamWriter sw = new StreamWriter(path))
                    {
                        using (StreamReader sr = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(Assembly.GetExecutingAssembly().GetManifestResourceNames().Single(str => str.EndsWith("herosiegeX.pas")))))
                        {
                            sw.Write(sr.ReadToEnd());
                        }
                    }


                }

                var parser = new FileIniDataParser();
                IniData data = parser.ReadFile(path);

                // Hero
                data["0"]["class"] = $"\"{this.HeroInfo.Class.ID}.000000\"";
                data["0"]["chaos_towers_cleared"] = $"\"{this.HeroInfo.ChaosTower}.000000\"";
                data["0"]["name"] = $"\"{MainWindow.INSTANCE.textBoxHeroName.Text}\"";
                data["0"]["level"] = $"\"{this.HeroInfo.Level}.000000\"";
                data["0"]["herolevel"] = $"\"{this.HeroInfo.HeroLevel}.000000\"";
                data["0"]["hardcore"] = $"\"{Convert.ToInt32(this.HeroInfo.Hardcore)}.000000\"";
                data["0"]["wormhole_level"] = $"\"{this.HeroInfo.WormholeLevel}.000000\"";

                // Inventory
                data.Sections.RemoveSection("inventory_list");
                data.Sections.AddSection("inventory_list");
                int y = 0;
                foreach (Item item in this.Inventory.InventoryItems)
                {
                    data["inventory_list"].AddKey("inventory_list" + y, item.GetItemString());
                    y++;
                }

                if (this.Inventory.UnknownItems != null)
                {
                    foreach (string s in this.Inventory.UnknownItems)
                    {
                        data["inventory_list"].AddKey("inventory_list" + y, s);
                        y++;
                    }
                }

                // Equipment
                data.Sections.RemoveSection("inventory");
                data.Sections.AddSection("inventory");
                foreach (EquipmentSlot slot in this.Inventory.Equipment.GetEquipmentList())
                    data["inventory"].AddKey($"inventory{slot.Slot.ID}", slot.Item.GetItemString());

                // Mercenary Equipment
                data.Sections.RemoveSection("minion_inventory_melee");
                data.Sections.RemoveSection("minion_inventory_ranged");

                foreach (EquipmentSlot slot in this.Mercenaries.Melee.Equipment.GetEquipmentList())
                    data["minion_inventory_melee"].AddKey($"inventory{slot.Slot.ID}", slot.Item.GetItemString());

                foreach (EquipmentSlot slot in this.Mercenaries.Ranged.Equipment.GetEquipmentList())
                    data["minion_inventory_ranged"].AddKey($"inventory{slot.Slot.ID}", slot.Item.GetItemString());

                // Relics
                foreach (KeyData key in data.Sections["0"])
                    if (key.KeyName.Contains("item"))
                        data["0"].RemoveKey(key.KeyName);

                foreach (Relic relic in this.Relics)
                {
                    if (relic.Type.Name == "Active")
                        data["0"].AddKey("active_item", $"\"{relic.ID}.000000\"");
                    data["0"].AddKey($"item{relic.ID}", "\"1.000000\"");
                }

                // Ubers
                data.Sections.RemoveSection("3");
                data.Sections.AddSection("3");
                foreach (Uber uber in this.Inventory.UberItems)
                    data["3"].AddKey($"uber{uber.ID}", $"\"{uber.Amount}.000000\"");

                // Runes
                data.Sections.RemoveSection("runes");
                data.Sections.AddSection("runes");
                foreach (Rune rune in this.Inventory.Runes)
                    data["runes"].AddKey($"rune_list{rune.IngameID}", $"\"{rune.IngameID}|{rune.Amount}\"");

                // Talents
                foreach (Talent talent in this.HeroInfo.Class.Talents.TalentList)
                    data["0"][$"talent_{talent.ID}"] = $"\"{talent.Points}.000000\"";

                foreach (ActiveTalent activeTalent in this.HeroInfo.ActiveTalents)
                    data["0"][$"active_talent_{activeTalent.ID}"] = activeTalent.Talent == null ? "\"0.000000\"" : $"\"{this.HeroInfo.Class.Talents.GetActiveTalents().IndexOf(activeTalent.Talent) + 1}\"";

                // Hero Talents
                foreach (KeyData key in data.Sections["0"])
                    if (key.KeyName.StartsWith("hero_talent")) data["0"].RemoveKey(key.KeyName);

                foreach (HeroTalent talent in this.HeroInfo.GetFilteredHeroTalents())
                    data["0"].AddKey($"hero_talent_{talent.ID}", $"\"{talent.SubID + 1}.000000\"");

                // Mercenary Talents
                foreach (KeyData key in data.Sections["0"])
                    if (key.KeyName.StartsWith("minion_talent")) data["0"].RemoveKey(key.KeyName);

                foreach (MercenaryTalent talent in this.Mercenaries.Talents.AllTalents)
                {
                    string x = talent.Type == "Offensive" ? "off" : "def";
                    if (talent.MercenaryID == 0 && this.Mercenaries.GetSelected() == 1 && talent.Type == "Offensive") continue;
                    int points = talent.MercenaryID == this.Mercenaries.GetSelected() ? talent.Points : 0;
                    data["0"].AddKey($"minion_talent_{x}{talent.ID}", $"\"{points}.000000\"");
                }

                IniData parsedINIDataToBeSaved = data;
                parser.WriteFile(Environment.GetEnvironmentVariable("LocalAppData") + $@"\Hero_Siege\save_folder\herosiege{this.ID}.pas", parsedINIDataToBeSaved);

                // Shop
                try
                {
                    Shop shop = MainWindow.INSTANCE.SaveFileHandler.Shop;
                    data = parser.ReadFile(Environment.GetEnvironmentVariable("LocalAppData") + @"\Hero_Siege\save_folder\shop.ini");

                    data["shop"]["currency"] = $"\"{shop.Rubies}.000000\"";

                    if (this.HeroInfo.Hardcore)
                    {
                        data["gold"]["gold_hc"] = $"\"{shop.Gold_HC}.000000\"";
                        data["mining"]["mining_hc"] = $"\"{shop.MiningLevel_HC}.000000\"";
                    }
                    else
                    {
                        data["gold"]["gold"] = $"\"{shop.Gold}.000000\"";
                        data["mining"]["mining"] = $"\"{shop.MiningLevel}.000000\"";
                    }

                    data["minion"]["companion"] = $"\"{shop.CompanionID}.000000\"";
                    data["minion"]["companion_name"] = $"\"{shop.CompanionName}\"";
                    data["minion"]["melee_name"] = $"\"{shop.MercenaryName_Melee}\"";
                    data["minion"]["ranged_name"] = $"\"{shop.MercenaryName_Ranged}\"";
                    data["minion"]["active"] = $"\"{Convert.ToInt32(shop.isMercenaryActive)}.000000\"";
                    data["minion"]["type"] = $"\"{this.Mercenaries.GetSelected()}.000000\"";

                    parsedINIDataToBeSaved = data;
                    parser.WriteFile(Environment.GetEnvironmentVariable("LocalAppData") + @"\Hero_Siege\save_folder\shop.ini", parsedINIDataToBeSaved);
                }
                catch { }


                // Stash
                try
                {
                    Shop shop2 = MainWindow.INSTANCE.SaveFileHandler.Shop;
                    data = parser.ReadFile(Environment.GetEnvironmentVariable("LocalAppData") + @"\Hero_Siege\save_folder\stash.pas");

                    foreach (KeyData key in data["0"])
                        if (!key.KeyName.StartsWith("stash_reset")) data["0"].RemoveKey(key.KeyName);

                    for (int i = 0; i < shop2.Stash.Count; i++)
                        data["0"].AddKey("stash_list" + i, shop2.Stash[i].GetItemString());

                    parsedINIDataToBeSaved = data;
                    parser.WriteFile(Environment.GetEnvironmentVariable("LocalAppData") + @"\Hero_Siege\save_folder\stash.pas", parsedINIDataToBeSaved);
                }
                catch { }
                Console.WriteLine();

            }
            catch { }
        }
    }



    public class SaveFileHandler
    {
        public ObservableCollection<SaveFile> SaveFiles { get; private set; }
        public Shop Shop { get; private set; }
        public SaveFile SelectedFile { get; set; }
        public SaveFile Copy { get; set; }

        private readonly string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Hero_Siege\save_folder\";

        public SaveFileHandler()
        {
            this.GetSaveFiles();
            this.Shop = new Shop();
        }

        public void GetSaveFiles()
        {
            ObservableCollection<SaveFile> saveFiles = new ObservableCollection<SaveFile>();
            for (int i = 0; i < 36; i++)
            {
                HeroInfo heroInfo;
                // "X" = File doesn't exist, use placeholder
                heroInfo = File.Exists(path + @$"herosiege{i}.pas") ? this.GetHeroInfo(i.ToString()) : this.GetHeroInfo("X");
                saveFiles.Add(new SaveFile(i, path + @$"herosiege{i}.pas", heroInfo, null, null, null, !File.Exists(path + @$"herosiege{i}.pas")));

            }
            this.SaveFiles = saveFiles;
        }


        public void ReadSaveFile(SaveFile saveFile)
        {
            if (saveFile == null) return;
            this.SelectedFile = saveFile;
            this.SelectedFile.Inventory = File.Exists(path + @$"herosiege{saveFile.ID}.pas") ? this.GetInventory(saveFile.ID) : new Inventory(new Equipment(), new ObservableCollection<Item>(), new ObservableCollection<Uber>(), new ObservableCollection<Rune>());
            this.SelectedFile.Relics = File.Exists(path + @$"herosiege{saveFile.ID}.pas") ? this.GetRelics(saveFile.ID) : new ObservableCollection<Relic>();
            this.SelectedFile.Mercenaries = File.Exists(path + @$"herosiege{saveFile.ID}.pas") ? this.GetMercenaries(saveFile.ID) : new Mercenaries(new Mercenary("Melee", MainWindow.INSTANCE.MercenaryHandler.GetMercenaryTypeFromID(0), new Equipment(true), true), new Mercenary("Ranged", MainWindow.INSTANCE.MercenaryHandler.GetMercenaryTypeFromID(1), new Equipment(true), false), MainWindow.INSTANCE.MercenaryHandler.GetDefaultTalents());
            if (!File.Exists(path + @$"herosiege{saveFile.ID}.pas")) return;
            this.SelectedFile.HeroInfo = this.GetHeroInfo(saveFile.ID.ToString());
        }

        private Mercenaries GetMercenaries(int slotid)
        {
            var parser = new FileIniDataParser();
            IniData data = parser.ReadFile(path + @$"herosiege{slotid}.pas");

            List<MercenaryTalent> Talents = MainWindow.INSTANCE.MercenaryHandler.GetDefaultTalents();

            foreach (KeyData key in data["0"])
            {
                if (!key.KeyName.StartsWith("minion_talent_")) continue;
                string x = key.KeyName.Contains("def") ? "Defensive" : "Offensive";
                string s = x.Remove(3).ToLower();
                foreach (MercenaryTalent talent in Talents)
                    if (talent.Type == x && talent.ID == Convert.ToInt32(key.KeyName.Replace($"minion_talent_{s}", string.Empty)))
                        talent.Points = (int)Util.FormatString(key.Value);
            }

            Equipment equipmentMelee = new Equipment();
            equipmentMelee.SetToMercenary();
            Equipment equipmentRanged = new Equipment();
            equipmentRanged.SetToMercenary();
            data = parser.ReadFile(path + @$"herosiege{slotid}.pas");
            foreach (KeyData key in data["minion_inventory_melee"])
            {
                int slot = Int32.Parse(key.KeyName.Trim("inventory".ToCharArray()));
                if (slot != 3 && slot != 1 && slot != 0 && slot != 7) continue;
                Item item = MainWindow.INSTANCE.ItemHandler.ParseSaveString(key.Value);
                if (item == null) continue;
                equipmentMelee.EquipItem(item, slot, false);
            }
            foreach (KeyData key in data["minion_inventory_ranged"])
            {
                int slot = Int32.Parse(key.KeyName.Trim("inventory".ToCharArray()));
                if (slot != 3 && slot != 1 && slot != 0 && slot != 7) continue;
                Item item = MainWindow.INSTANCE.ItemHandler.ParseSaveString(key.Value);
                if (item == null) continue;
                equipmentRanged.EquipItem(item, slot, false);
            }


            Mercenary Melee = new Mercenary(this.Shop.MercenaryName_Melee, MainWindow.INSTANCE.MercenaryHandler.GetMercenaryTypeFromName("Melee"), equipmentMelee, MainWindow.INSTANCE.SaveFileHandler.Shop.SelectedMercenary == 0);
            Mercenary Ranged = new Mercenary(this.Shop.MercenaryName_Ranged, MainWindow.INSTANCE.MercenaryHandler.GetMercenaryTypeFromName("Ranged"), equipmentRanged, MainWindow.INSTANCE.SaveFileHandler.Shop.SelectedMercenary == 1);
            return new Mercenaries(Melee, Ranged, Talents);
        }

        private HeroInfo GetHeroInfo(string slotid)
        {
            try
            {
                var parser = new FileIniDataParser();
                IniData data = slotid == "X" ? parser.ReadData(new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(Assembly.GetExecutingAssembly().GetManifestResourceNames().Single(str => str.EndsWith("herosiegeX.pas"))))) : parser.ReadFile(path + @$"herosiege{slotid}.pas");

                Class heroClass = MainWindow.INSTANCE.ClassHandler.GetClassFromID((int)Util.FormatString(data["0"]["class"])).DeepCopy();
                List<HeroTalent> HeroTalents = MainWindow.INSTANCE.TalentHandler.GetHeroTalentList();
                List<ActiveTalent> ActiveTalents = MainWindow.INSTANCE.TalentHandler.GetEmptyActiveTalents();
                foreach (KeyData key in data.Sections["0"])
                {
                    try
                    {
                        if (key.KeyName.StartsWith("active_talent_"))
                        {
                            if (Util.FormatString(key.Value) == 0) continue;
                            int id = Int32.Parse(key.KeyName.Replace("active_talent_", String.Empty));
                            ActiveTalents[id].Talent = heroClass.Talents.GetActiveTalents()[(int)Util.FormatString(key.Value) - 1];
                        }


                        if (key.KeyName.Contains("hero_talent"))
                        {
                            foreach (HeroTalent talent in HeroTalents)
                                if (talent.ID == Convert.ToInt32(key.KeyName.Remove(0, 12)) && talent.SubID + 1 == Convert.ToInt32(Util.FormatString(key.Value)))
                                    talent.Selected = true;

                        }
                        if (key.KeyName.StartsWith("talent_") && !key.KeyName.Contains("reset"))
                            heroClass.Talents.GetTalentFromID(Int32.Parse(key.KeyName.Remove(0, 7))).Points = (int)Util.FormatString(key.Value);
                    }
                    catch { }
                }

                string name = slotid == "X" || data["0"]["name"] == null ? MainWindow.INSTANCE.ConfigHandler.Config.NewChar.Name : data["0"]["name"].Trim('"');
                int level = slotid == "X" || data["0"]["level"] == null ? MainWindow.INSTANCE.ConfigHandler.Config.NewChar.Level : (int)Util.FormatString(data["0"]["level"]);
                int herolevel = slotid == "X" || data["0"]["herolevel"] == null ? MainWindow.INSTANCE.ConfigHandler.Config.NewChar.HeroLevel : Util.FormatString(data["0"]["herolevel"]);
                int wormhole = slotid == "X" || data["0"]["wormhole_level"] == null ? MainWindow.INSTANCE.ConfigHandler.Config.NewChar.WormholeLevel : Util.FormatString(data["0"]["wormhole_level"]);
                int ct = slotid == "X" || data["0"]["chaos_towers_cleared"] == null ? MainWindow.INSTANCE.ConfigHandler.Config.NewChar.ChaosTower : Util.FormatString(data["0"]["chaos_towers_cleared"]);
                bool hardcore = slotid == "X" || data["0"]["hardcore"] == null ? MainWindow.INSTANCE.ConfigHandler.Config.NewChar.Hardcore : Util.FormatString(data["0"]["hardcore"]) == 1;



                return new HeroInfo(slotid == "X" ? MainWindow.INSTANCE.ConfigHandler.Config.NewChar.Class.DeepCopy() : heroClass, name, level, herolevel, wormhole, ct, hardcore, HeroTalents, ActiveTalents);
            }
            catch
            {
                var result = System.Windows.MessageBox.Show($"Couldn't read the savefile herosige{slotid}.pas.\r\nDo you want to replace it with a new savefile and continue?", "Error", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Exclamation);
                if (result == System.Windows.MessageBoxResult.Yes)
                {
                    using (StreamWriter sw = new StreamWriter(path + $"herosiege{slotid}.pas"))
                    {
                        using (StreamReader sr = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(Assembly.GetExecutingAssembly().GetManifestResourceNames().Single(str => str.EndsWith("herosiegeX.pas")))))
                        {
                            sw.Write(sr.ReadToEnd());
                        }
                    }
                    return GetHeroInfo(slotid);
                }
                else
                {
                    Environment.Exit(0);
                    return null;
                }
            }
        }

        private Inventory GetInventory(int slotid)
        {
            Equipment equipment = new Equipment();
            ObservableCollection<Item> inventory = new ObservableCollection<Item>();
            ObservableCollection<Uber> ubers = new ObservableCollection<Uber>();
            List<string> UnknownItems = new List<string>();
            foreach (Uber uber in MainWindow.INSTANCE.UberHandler.Ubers)
                ubers.Add(uber.DeepCopy());
            ObservableCollection<Rune> runes = new ObservableCollection<Rune>();
            foreach (Rune rune in MainWindow.INSTANCE.RuneHandler.RunesFiltered)
                runes.Add(rune.DeepCopy());


            var parser = new FileIniDataParser();
            IniData data = parser.ReadFile(path + @$"herosiege{slotid}.pas");

            foreach (KeyData key in data["inventory"])
            {
                Item item = MainWindow.INSTANCE.ItemHandler.ParseSaveString(key.Value);
                if (item == null) { UnknownItems.Add(key.Value); continue; }
                equipment.EquipItem(item, Int32.Parse(key.KeyName.Trim("inventory".ToCharArray())), false);
            }



            foreach (KeyData key in data["inventory_list"])
            {
                Item item = MainWindow.INSTANCE.ItemHandler.ParseSaveString(key.Value);
                if (item == null) { UnknownItems.Add(key.Value); continue; }
                inventory.Add(item);
            }



            foreach (KeyData key in data["3"])
                if (Int32.Parse(key.Value.Trim('"').Split('.')[0]) != 0)
                    foreach (Uber uber in ubers)
                    {
                        bool x = false;
                        if (uber.ID == Int32.Parse(key.KeyName.Trim("uber".ToCharArray())))
                            x = true;

                        if (x)
                        {
                            uber.Amount = Int32.Parse(key.Value.Trim('"').Split('.')[0]);
                            break;
                        }
                    }




            foreach (KeyData key in data["runes"])
            {
                string[] temp2 = key.Value.Split('|');
                foreach (Rune rune in runes)
                {
                    bool x = false;
                    if (rune.IngameID == Int32.Parse(temp2[0].Trim('"')))
                        x = true;

                    if (x)
                    {
                        rune.Amount = Int32.Parse(temp2[1].Trim('"'));
                        break;
                    }
                }


            }

            Inventory Inventory = new Inventory(equipment, inventory, ubers, runes);
            Inventory.UnknownItems = UnknownItems;
            return Inventory;
        }

        private ObservableCollection<Relic> GetRelics(int slotid)
        {
            List<Relic> relics = new List<Relic>();

            var parser = new FileIniDataParser();
            IniData data = parser.ReadFile(path + @$"herosiege{slotid}.pas");
            foreach (KeyData key in data["0"])
            {

                if (key.KeyName.Contains("item") && key.KeyName != "active_item")
                {
                    Relic relic = MainWindow.INSTANCE.RelicHandler.GetRelicFromID(Int32.Parse(key.KeyName.Trim("item".ToCharArray())));
                    if (relic == null) continue;
                    relics.Add(relic);
                }

            }
            relics = relics.OrderBy(o => o.ID).ToList();
            return new ObservableCollection<Relic>(relics);
        }
    }
}
