using HSeditor.Classes.Other;
using HSeditor.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace HSeditor.Classes.SaveFiles
{
    public class Shop
    {
        public int MiningLevel_HC { get; set; }
        public int MiningLevel { get; set; }
        public int Gold_HC { get; set; }
        public int Gold { get; set; }
        public int Rubies { get; set; }
        public string MercenaryName_Melee { get; set; }
        public string MercenaryName_Ranged { get; set; }
        public int SelectedMercenary { get; set; }
        public bool isMercenaryActive { get; set; }
        public string CompanionName { get; set; }
        public int CompanionID { get; set; }
        public List<Item> Stash { get; set; }

        private readonly string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Hero_Siege\hs2saves\";
        private readonly string _stashPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Hero_Siege\hseditor\saves\stash.hss";
        public Shop()
        {
            this.Refresh();
        }

        public void Refresh()
        {
            if (!File.Exists(path + @"shop.ini"))
            {
                using (StreamWriter sw = new StreamWriter(path + @"shop.ini"))
                {
                    using (StreamReader sr = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(Assembly.GetExecutingAssembly().GetManifestResourceNames().Single(str => str.EndsWith("shopX.ini")))))
                    {
                        sw.Write(sr.ReadToEnd());
                    }
                }
            }

            var parser = new FileIniDataParser();
            IniData data = parser.ReadFile(path + @"shop.ini");

            this.Rubies = data["shop"]["currency"] == null ? 0 : Util.Util.FormatString(data["shop"]["currency"]);
            this.Gold = data["gold"]["gold"] == null ? 0 : Util.Util.FormatString(data["gold"]["gold"]);
            this.Gold_HC = data["gold"]["gold_hc"] == null ? 0 : Util.Util.FormatString(data["gold"]["gold_hc"]);
            this.MiningLevel = data["mining"]["mining"] == null ? 0 : Util.Util.FormatString(data["mining"]["mining"]);
            this.MiningLevel_HC = data["mining"]["mining_hc"] == null ? 0 : Util.Util.FormatString(data["mining"]["mining_hc"]);
            this.MercenaryName_Melee = data["minion"]["melee_name"] == null ? "Melee_Minion" : data["minion"]["melee_name"].Trim('"');
            this.MercenaryName_Ranged = data["minion"]["ranged_name"] == null ? "Ranged_Minion" : data["minion"]["ranged_name"].Trim('"');
            this.isMercenaryActive = data["minion"]["active"] == null ? true : Util.Util.FormatString(data["minion"]["active"]) == 1;
            this.SelectedMercenary = data["minion"]["type"] == null ? 0 : Util.Util.FormatString(data["minion"]["type"]);
            this.CompanionName = data["minion"]["companion_name"] == null ? "Companion" : data["minion"]["companion_name"].Trim('"');
            this.CompanionID = data["minion"]["companion"] == null ? 0 : Util.Util.FormatString(data["minion"]["companion"]);

            if (!File.Exists(path + @"stash.hss"))
            {
                StreamWriter sr = new StreamWriter(path + @"stash.hss");
            }

            this.Stash = new List<Item>();

            string stash = File.Exists(_stashPath) ? File.ReadAllText(_stashPath) : "";

            if (stash != "")
            {
                JObject invObj = JObject.Parse(stash);
                for (int i = 1; i < 6; i++)
                {
                    try
                    {
                        foreach (JObject item in invObj[$"stash_tab_{i}"])
                        {
                            Item item2 = MainWindow.INSTANCE.ItemHandler.ParseJSONObject(item);
                            if (item2 == null) continue;
                            item2.Inv = (ItemHandler.InvType)(i + MainWindow.INSTANCE.ItemHandler.StashIndex);
                            item2.SaveItem = item;
                            this.Stash.Add(item2);
                        }
                    }
                    catch { }
                }
            }
        }

        public void AddItemToStash(Item item, bool findPlace = true)
        {
            item = item.DeepCopy();
            item.Inv = MainWindow.INSTANCE.SelectedStash;
            if (findPlace || item.InvPos == null)
            {
                InventoryBox box = MainWindow.INSTANCE.InventoryBoxHandler.FindSpace(item, MainWindow.INSTANCE.InventoryBoxHandler.StashBoxes);
                if (box == null) return;
                item.InvPos = box.Position;
            }
            if (item.Inv == ItemHandler.InvType.Stash0)
                MainWindow.INSTANCE.SaveFileHandler.SelectedFile.Inventory.StashItems.Add(item);
            else
                this.Stash.Add(item);
        }

    }
}
