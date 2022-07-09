using HSeditor.Model;
using System;
using System.Collections.ObjectModel;
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
        public ObservableCollection<Item> Stash { get; set; }

        private readonly string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Hero_Siege\save_folder\";
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

            if (!File.Exists(path + @"stash.pas"))
            {
                StreamWriter sr = new StreamWriter(path + @"stash.pas");
                sr.WriteLine("[0]");
                sr.WriteLine("stash_reset = \"0.000000\"");
                sr.Close();
            }

            this.Stash = new ObservableCollection<Item>();

            data = parser.ReadFile(path + @"stash.pas");
            foreach (KeyData key in data["0"])
                if (key.KeyName.StartsWith("stash_list"))
                {
                    Item item = MainWindow.INSTANCE.ItemHandler.ParseSaveString(key.Value);
                    if (item == null) continue;
                    item.Stash = true;
                    this.Stash.Add(item);
                }
        }

        public void AddItemToStash(Item item)
        {
            item.Stash = true;
            this.Stash.Add(item);
        }

    }
}
