using HSeditor.Classes;
using HSeditor.Classes.Items;
using HSeditor.Classes.Other;
using HSeditor.Classes.SaveFiles;
using HSeditor.Classes.Util;
using HSeditor.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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
        public HeroInfo HeroInfo { get; set; }
        public string Sprite { get; set; }
        public string Color { get; set; }
        public bool NewChar { get; private set; }

        public SaveFile(int ID, string Path, HeroInfo HeroInfo, Inventory Inventory = null, bool newchar = false)
        {
            this.ID = ID;
            this.Path = Path;
            this.HeroInfo = HeroInfo;
            this.Sprite = newchar ? Environment.CurrentDirectory + @$"\Sprites\Classes\NewChar.png" : HeroInfo.Class.Sprite;
            this.Color = newchar ? "#99AAB5" : this.ID % 2 != 0 ? "#bababa" : "#F6F6F6";
            this.NewChar = newchar;
            if (!newchar) this.SetProperties();
        }

        public void SetProperties()
        {
            this.Inventory = Inventory == null ? new Inventory(new Equipment(), new List<Item>(), new List<Item>()) : Inventory;
        }

        public void Save()
        {
            MainWindow.INSTANCE.SaveFileHandler.Save(this);
        }

        public void Delete()
        {
            MainWindow.INSTANCE.SaveFileHandler.Delete(this);
        }
    }
    public class SaveFileHandler
    {
        public List<SaveFile> SaveFiles { get; private set; }
        public Shop Shop { get; private set; }
        public SaveFile SelectedFile { get; set; }
        public SaveFile Copy { get; set; }

        private readonly string _savePath1 = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Hero_Siege\hseditor\saves\";
        private readonly string _savePath2 = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Hero_Siege\hs2saves\";
        private readonly string _shopPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Hero_Siege\hs2saves\shop.ini";
        private readonly string _stashPath1 = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Hero_Siege\hseditor\saves\stash.hss";
        private readonly string _stashPath2 = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Hero_Siege\hs2saves\stash.hss";


        public SaveFileHandler()
        {
            this.GetSaveFiles();
            this.Shop = new Shop();
        }

        public void GetSaveFiles()
        {
            List<SaveFile> saveFiles = new List<SaveFile>();
            for (int i = 0; i < 36; i++)
            {
                HeroInfo heroInfo;
                // "X" = File doesn't exist, use placeholder
                heroInfo = File.Exists(_savePath1 + @$"herosiege{i}.hss") ? this.GetHeroInfo(i.ToString()) : null;
                saveFiles.Add(new SaveFile(i, _savePath1 + @$"herosiege{i}.hss", heroInfo, null, heroInfo == null));
            }
            this.SaveFiles = saveFiles;
        }

        public void Save(SaveFile saveFile)
        {
            try
            {
                var savePath1 = _savePath1 + @$"herosiege{saveFile.ID}.hss";
                var savePath2 = _savePath2 + @$"herosiege{saveFile.ID}.hss";

                if (!File.Exists(savePath1))
                {
                    using (StreamWriter sw = new StreamWriter(savePath1))
                    {
                        using (StreamReader sr = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(Assembly.GetExecutingAssembly().GetManifestResourceNames().Single(str => str.EndsWith("herosiegeX.pas")))))
                        {
                            sw.Write(sr.ReadToEnd());
                        }
                    }
                }

                var parser = new FileIniDataParser();
                IniData data = parser.ReadFile(savePath1);

                // Hero
                data["0"]["class"] = $"\"{saveFile.HeroInfo.Class.ID}.000000\"";
                data["0"]["chaos_towers_cleared"] = $"\"{saveFile.HeroInfo.ChaosTower}.000000\"";
                data["0"]["name"] = saveFile.NewChar ? "\"" + saveFile.HeroInfo.Name + "\"" : $"\"{MainWindow.INSTANCE.textBoxHeroName.Text}\"";
                data["0"]["level"] = $"\"{saveFile.HeroInfo.Level}.000000\"";
                data["0"]["herolevel"] = $"\"{saveFile.HeroInfo.HeroLevel}.000000\"";
                data["0"]["hardcore"] = $"\"{Convert.ToInt32(saveFile.HeroInfo.Hardcore)}.000000\"";
                data["0"]["wormhole_level"] = $"\"{saveFile.HeroInfo.WormholeLevel}.000000\"";
                Random rnd = new Random();

                // Inventory
                List<JObject> list1 = new List<JObject>();
                List<JObject> list2 = new List<JObject>();
                List<JObject> list3 = new List<JObject>();
                List<JObject> list4 = new List<JObject>();
                List<JObject> list5 = new List<JObject>();
                List<JObject> list6 = new List<JObject>();
                foreach (Item item in saveFile.Inventory.InventoryItems)
                {
                    JObject itemObj = item.GetItemObject();
                    itemObj["seed"] = (int)itemObj["seed"] == -1 ? rnd.Next(0, 10000000) : itemObj["seed"];
                    itemObj["timestamp"] = rnd.Next(0, 100000000);
                    switch (item.Inv)
                    {
                        case ItemHandler.InvType.Main:
                            list1.Add(itemObj);
                            break;
                        case ItemHandler.InvType.Extra1:
                            list2.Add(itemObj);
                            break;
                        case ItemHandler.InvType.Extra2:
                            list3.Add(itemObj);
                            break;
                        case ItemHandler.InvType.Extra3:
                            list4.Add(itemObj);
                            break;
                        case ItemHandler.InvType.Socketables:
                            list5.Add(itemObj);
                            break;
                        case ItemHandler.InvType.Potion:
                            list6.Add(itemObj);
                            break;
                    }

                }

                    SaveInventory saveInventory = new SaveInventory(list1.ToArray(), list2.ToArray(), list3.ToArray(), list4.ToArray(), list5.ToArray(), list6.ToArray());
                    string s = JsonConvert.SerializeObject(saveInventory, Formatting.Indented);
                if (MainWindow.INSTANCE.checkbox_savechar.IsChecked == true)
                {
                    using (StreamWriter sw2 = new StreamWriter(_savePath1 + $"inventory_order_{saveFile.ID}.hss"))
                    {
                        sw2.Write(s);
                    }
                    File.Copy(_savePath1 + $"inventory_order_{saveFile.ID}.hss", _savePath2 + $"inventory_order_{saveFile.ID}.hss", true);

                    // Equipment
                    data.Sections.RemoveSection("inventory");
                    data.Sections.AddSection("inventory");
                    foreach (EquipmentSlot slot in saveFile.Inventory.Equipment.GetEquipmentList())
                    {
                        slot.Item.RollID = slot.Item.RollID == -1 ? rnd.Next(0, 1001) : slot.Item.RollID;
                        slot.Item.SaveItem["timestamp"] = rnd.Next(0, 100000000);
                        data["inventory"].AddKey($"inventory{slot.ID}", slot.Item.GetItemString());
                    }

                    IniData parsedINIDataToBeSaved = data;

                    parser.WriteFile(savePath1, parsedINIDataToBeSaved);
                    File.Copy(savePath1, savePath2, true);

                    try
                    {
                        Shop shop = MainWindow.INSTANCE.SaveFileHandler.Shop;
                        data = parser.ReadFile(_shopPath);

                        data["shop"]["currency"] = $"\"{shop.Rubies}.000000\"";

                        if (saveFile.HeroInfo.Hardcore)
                        {
                            data["gold"]["gold_hc"] = $"\"{shop.Gold_HC}.000000\"";
                            data["mining"]["mining_hc"] = $"\"{shop.MiningLevel_HC}.000000\"";
                        }
                        else
                        {
                            data["gold"]["gold"] = $"\"{shop.Gold}.000000\"";
                            data["mining"]["mining"] = $"\"{shop.MiningLevel}.000000\"";
                        }

                        parsedINIDataToBeSaved = data;
                        parser.WriteFile(_shopPath, parsedINIDataToBeSaved);
                    }
                    catch { }
                }

                // Stash
                if(MainWindow.INSTANCE.checkbox_savestash.IsChecked==true)
                try
                {
                    list1 = new List<JObject>();
                    list2 = new List<JObject>();
                    list3 = new List<JObject>();
                    list4 = new List<JObject>();
                    list5 = new List<JObject>();
                    foreach (Item item in MainWindow.INSTANCE.SaveFileHandler.Shop.Stash)
                    {
                        JObject itemObj = item.GetItemObject();
                        itemObj["seed"] = (int)itemObj["seed"] == -1 ? rnd.Next(0, 10000) : itemObj["seed"];
                        itemObj["timestamp"] = rnd.Next(0, 100000000);
                        switch (item.Inv)
                        {
                            case ItemHandler.InvType.Stash1:
                                list1.Add(itemObj);
                                break;
                            case ItemHandler.InvType.Stash2:
                                list2.Add(itemObj);
                                break;
                            case ItemHandler.InvType.Stash3:
                                list3.Add(itemObj);
                                break;
                            case ItemHandler.InvType.Stash4:
                                list4.Add(itemObj);
                                break;
                            case ItemHandler.InvType.Stash5:
                                list5.Add(itemObj);
                                break;
                        }

                    }
                    SaveStash saveStash = new SaveStash(list1.ToArray(), list2.ToArray(), list3.ToArray(), list4.ToArray(), list5.ToArray(), new JObject[0]);
                    s = JsonConvert.SerializeObject(saveStash, Formatting.Indented);

                    using (StreamWriter sw2 = new StreamWriter(_stashPath1))
                    {
                        sw2.Write(s);
                    }
                    File.Copy(_stashPath1, _stashPath2, true);
                }
                catch { }
                Console.WriteLine();

            }
            catch { }
        }               

        public void Delete(SaveFile saveFile)
        {
            File.Delete(_savePath1 + $"herosiege{saveFile.ID}.hss");
            File.Delete(_savePath2 + $"herosiege{saveFile.ID}.hss");
            File.Delete(_savePath1 + $"inventory_order_{saveFile.ID}.hss");
            File.Delete(_savePath2 + $"inventory_order_{saveFile.ID}.hss");
        }

        public void ReadSaveFile(SaveFile saveFile, bool select = true)
        {
            if (saveFile == null) return;
            if (select)
                this.SelectedFile = saveFile;
            saveFile.Inventory = File.Exists(_savePath1 + @$"herosiege{saveFile.ID}.hss") ? this.GetInventory(saveFile.ID) : new Inventory(new Equipment(), new List<Item>(), new List<Item>());
            if (!File.Exists(_savePath1 + @$"herosiege{saveFile.ID}.hss")) return;
            saveFile.HeroInfo = this.GetHeroInfo(saveFile.ID.ToString());
        }

        public HeroInfo GetHeroInfo(string slotid)
        {
            try
            {
                var parser = new FileIniDataParser();
                IniData data = slotid == "X" ? parser.ReadData(new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(Assembly.GetExecutingAssembly().GetManifestResourceNames().Single(str => str.EndsWith("herosiegeX.pas"))))) : parser.ReadFile(_savePath1 + $"herosiege{slotid}.hss");

                Class heroClass = MainWindow.INSTANCE.ClassHandler.GetClassFromID((int)Util.FormatString(data["0"]["class"])).DeepCopy();

                string name = slotid == "X" || data["0"]["name"] == null ? "New Char" : data["0"]["name"].Trim('"');
                int level = slotid == "X" || data["0"]["level"] == null ? 100 : (int)Util.FormatString(data["0"]["level"]);
                int herolevel = slotid == "X" || data["0"]["herolevel"] == null ? 0 : Util.FormatString(data["0"]["herolevel"]);
                int wormhole = slotid == "X" || data["0"]["wormhole_level"] == null ? 0 : Util.FormatString(data["0"]["wormhole_level"]);
                int ct = slotid == "X" || data["0"]["chaos_towers_cleared"] == null ? 0 : Util.FormatString(data["0"]["chaos_towers_cleared"]);
                bool hardcore = slotid == "X" || data["0"]["hardcore"] == null ? false : Util.FormatString(data["0"]["hardcore"]) == 1;



                return new HeroInfo(slotid == "X" ? MainWindow.INSTANCE.ClassHandler.GetClassFromName("Viking").DeepCopy() : heroClass, name, level, herolevel, wormhole, ct, hardcore);
            }
            catch
            {
                var result = System.Windows.MessageBox.Show($"Couldn't read the savefile herosige{slotid}.hss.\r\nDo you want to replace it with a new savefile and continue?", "Error", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Exclamation);
                if (result == System.Windows.MessageBoxResult.Yes)
                {
                    using (StreamWriter sw = new StreamWriter(_savePath1 + $"herosiege{slotid}.hss"))
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
            List<Item> inventory = new List<Item>();
            List<Item> stash = new List<Item>();
            List<string> UnknownItems = new List<string>();

            var parser = new FileIniDataParser();
            IniData data = parser.ReadFile(_savePath1 + $"herosiege{slotid}.hss");

            string inv = File.Exists(this._savePath1 + $"inventory_order_{slotid}.hss") ? File.ReadAllText(this._savePath1 + $"inventory_order_{slotid}.hss") : "";

            if (inv != "")
            {
                JObject invObj = JObject.Parse(inv);
                for (int i = 0; i < 4; i++)
                {
                    if (!invObj.ContainsKey($"inventory_tab_{i}")) continue;
                    foreach (JObject item in invObj[$"inventory_tab_{i}"])
                    {
                        Item item2 = MainWindow.INSTANCE.ItemHandler.ParseJSONObject(item);
                        if (item2 == null) { UnknownItems.Add(item.ToString()); continue; }
                        item2.Inv = (ItemHandler.InvType)(i + 2);
                        item2.SaveItem = item;
                        inventory.Add(item2);
                    }
                }

                if (invObj.ContainsKey("inventory_socket_tab"))
                {
                    foreach (JObject item in invObj["inventory_socket_tab"])
                    {
                        Item item2 = MainWindow.INSTANCE.ItemHandler.ParseJSONObject(item);
                        if (item2 == null) { UnknownItems.Add(item.ToString()); continue; }
                        item2.Inv = ItemHandler.InvType.Socketables;
                        item2.SaveItem = item;
                        inventory.Add(item2);
                    }
                }

                if (invObj.ContainsKey("inventory_potion_tab"))
                {
                    foreach (JObject item in invObj["inventory_potion_tab"])
                    {
                        Item item2 = MainWindow.INSTANCE.ItemHandler.ParseJSONObject(item);
                        if (item2 == null) { UnknownItems.Add(item.ToString()); continue; }
                        item2.Inv = ItemHandler.InvType.Potion;
                        item2.SaveItem = item;
                        inventory.Add(item2);
                    }
                }
            }

            foreach (KeyData key in data["inventory"])
            {
                Item item = MainWindow.INSTANCE.ItemHandler.ParseSaveString(key.Value);
                if (item == null) { UnknownItems.Add(key.Value); continue; }
                item.SaveItem = JObject.Parse(System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(key.Value.Replace("'", String.Empty).Replace("\"", String.Empty))));
                equipment.EquipItem(item, Int32.Parse(key.KeyName.Trim("inventory".ToCharArray())));
            }

            Inventory Inventory = new Inventory(equipment, inventory, stash);
            Inventory.UnknownItems = UnknownItems;
            return Inventory;
        }
    }
}
