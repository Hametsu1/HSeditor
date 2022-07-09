using HSeditor.Classes.Items;
using HSeditor.Classes.Merc;
using HSeditor.Classes.SaveFiles;
using HSeditor.Model;
using HSeditor.SaveFiles;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace HSeditor.Classes.Other
{
    public class Build
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Author { get; private set; }
        public string FileName { get; private set; }
        public SaveFile SaveFile { get; set; }


        public Build(string Name, string Description, string AuthorName, string FileName)
        {
            this.Name = Name;
            this.Description = Description;
            this.Author = AuthorName;
            this.FileName = FileName;
        }
    }

    public class BuildHandler
    {
        public List<Build> Builds = new List<Build>();
        public List<Build> OfflineBuilds = new List<Build>();
        public bool failedConnection { get; private set; }
        private readonly string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Hero_Siege\hseditor\builds\";


        public BuildHandler()
        {
            this.GetBuilds();
            this.GetOfflineBuilds();
        }

        public void GetBuilds()
        {
            try
            {
                this.Builds.Clear();
                WebRequest request = WebRequest.Create("http://hseditor.com/api/templates.ini");
                request.Timeout = 1000;

                Stream stream = request.GetResponse().GetResponseStream();
                StreamReader reader = new StreamReader(stream);

                var parser = new FileIniDataParser();
                IniData data = parser.ReadData(reader);

                foreach (SectionData section in data.Sections)
                {
                    Build build = new Build(section.Keys["title"].Trim('"'), section.Keys["description"].Trim('"'), section.Keys["author"].Trim('"'), section.Keys["filename"].Trim('"'));
                    build.SaveFile = this.GetSaveFile(build.FileName);
                    this.Builds.Add(build);
                }
                failedConnection = false;
            }
            catch
            {
                failedConnection = true;
            }

        }

        public void GetOfflineBuilds()
        {
            this.OfflineBuilds.Clear();
            foreach (var file in new DirectoryInfo(this.path).GetFiles("*.build"))
            {
                try
                {
                    var parser = new FileIniDataParser();
                    IniData data = parser.ReadFile(file.FullName);
                    Build build = new Build(data["build_info"]["name"] == null ? System.IO.Path.GetFileNameWithoutExtension(file.FullName) : data["build_info"]["name"], data["build_info"]["description"] == null ? "This is a local build" : data["build_info"]["description"], "Local", file.FullName);
                    HeroInfo heroInfo = this.GetHeroInfo(data);
                    Inventory inventory = this.GetInventory(data);
                    Mercenaries mercenaries = this.GetMercenaries(data);
                    ObservableCollection<Relic> relics = this.GetRelics(data);
                    if (inventory == null || heroInfo == null || mercenaries == null || relics == null) continue;
                    build.SaveFile = new SaveFile(heroInfo, inventory, mercenaries, relics);
                    this.OfflineBuilds.Add(build);
                }
                catch { }
            }
        }

        public Mercenaries GetMercenaries(IniData data)
        {
            try
            {
                List<MercenaryTalent> talents = MainWindow.INSTANCE.MercenaryHandler.GetDefaultTalents();

                foreach (KeyData key in data["mercenary"])
                {
                    if (!key.KeyName.StartsWith("minion_talent_")) continue;
                    string x = key.KeyName.Contains("def") ? "Defensive" : "Offensive";
                    string s = x.Remove(3).ToLower();
                    foreach (MercenaryTalent talent in talents)
                        if (talent.Type == x && talent.ID == Convert.ToInt32(key.KeyName.Replace($"minion_talent_{s}", string.Empty)))
                            talent.Points = Convert.ToInt32(key.Value);
                }

                Equipment equipmentMelee = new Equipment();
                equipmentMelee.SetToMercenary();
                Equipment equipmentRanged = new Equipment();
                equipmentRanged.SetToMercenary();

                foreach (KeyData key in data["minion_inventory_melee"])
                {
                    int slot = Int32.Parse(key.KeyName.Trim("inventory".ToCharArray()));
                    if (slot != 3 && slot != 1 && slot != 0 && slot != 7) continue;
                    equipmentMelee.EquipItem(MainWindow.INSTANCE.ItemHandler.ParseSaveString(key.Value), slot, false);
                }
                foreach (KeyData key in data["minion_inventory_ranged"])
                {
                    int slot = Int32.Parse(key.KeyName.Trim("inventory".ToCharArray()));
                    if (slot != 3 && slot != 1 && slot != 0 && slot != 7) continue;
                    equipmentRanged.EquipItem(MainWindow.INSTANCE.ItemHandler.ParseSaveString(key.Value), slot, false);
                }


                Mercenary Melee = new Mercenary("", MainWindow.INSTANCE.MercenaryHandler.GetMercenaryTypeFromName("Melee"), equipmentMelee, Util.Util.FormatString(data["mercenary"]["type"]) == 0);
                Mercenary Ranged = new Mercenary("", MainWindow.INSTANCE.MercenaryHandler.GetMercenaryTypeFromName("Ranged"), equipmentRanged, Util.Util.FormatString(data["mercenary"]["type"]) == 1);
                return new Mercenaries(Melee, Ranged, talents);
            }
            catch
            {
                return null;
            }
        }

        private SaveFile GetSaveFile(string filename)
        {
            WebClient client = new WebClient();
            Stream stream = client.OpenRead($"http://hseditor.com/uploads/templates/{filename}.build");
            StreamReader reader = new StreamReader(stream);
            var parser = new FileIniDataParser();
            IniData data = parser.ReadData(reader);
            HeroInfo heroInfo = this.GetHeroInfo(data);
            Inventory inventory = this.GetInventory(data);
            Mercenaries mercenaries = this.GetMercenaries(data);
            ObservableCollection<Relic> relics = this.GetRelics(data);
            if (inventory == null || heroInfo == null || mercenaries == null || relics == null) return null;
            return new SaveFile(heroInfo, inventory, mercenaries, relics);
        }

        private HeroInfo GetHeroInfo(IniData data)
        {
            try
            {
                Class Class = MainWindow.INSTANCE.ClassHandler.GetClassFromID(Int32.Parse(data["hero_info"]["class"]));
                List<HeroTalent> HeroTalents = MainWindow.INSTANCE.TalentHandler.GetHeroTalentList();
                List<ActiveTalent> ActiveTalents = MainWindow.INSTANCE.TalentHandler.GetEmptyActiveTalents();
                foreach (KeyData key in data.Sections["hero_info"])
                {
                    if (key.KeyName.Contains("hero_talent"))
                    {
                        foreach (HeroTalent talent in HeroTalents)
                            if (talent.ID == Convert.ToInt32(key.KeyName.Remove(0, 12)) && talent.SubID + 1 == Convert.ToInt32(Util.Util.FormatString(key.Value)))
                                talent.Selected = true;
                    }
                    if (key.KeyName.StartsWith("talent_") && !key.KeyName.Contains("reset"))
                        Class.Talents.GetTalentFromID(Int32.Parse(key.KeyName.Remove(0, 7))).Points = Util.Util.FormatString(key.Value);

                    if (key.KeyName.StartsWith("active_talent_"))
                    {
                        if (Convert.ToInt32(key.Value) == 0) continue;
                        int id = Int32.Parse(key.KeyName.Replace("active_talent_", String.Empty));
                        ActiveTalents[id].Talent = Class.Talents.GetActiveTalents()[Convert.ToInt32(key.Value) - 1];
                    }
                }

                return new HeroInfo(Class, Int32.Parse(data["hero_info"]["level"]), Int32.Parse(data["hero_info"]["hero_level"]), HeroTalents, ActiveTalents);
            }
            catch { return null; }
        }

        private Inventory GetInventory(IniData data)
        {
            Equipment equipment = new Equipment();
            ObservableCollection<Item> inventory = new ObservableCollection<Item>();

            foreach (KeyData key in data["inventory"])
                equipment.EquipItem(MainWindow.INSTANCE.ItemHandler.ParseSaveString(key.Value), Int32.Parse(key.KeyName.Trim("inventory".ToCharArray())));

            foreach (KeyData key in data["inventory_list"])
                inventory.Add(MainWindow.INSTANCE.ItemHandler.ParseSaveString(key.Value));

            return new Inventory(equipment, inventory);
        }

        private ObservableCollection<Relic> GetRelics(IniData data)
        {
            ObservableCollection<Relic> relics = new ObservableCollection<Relic>();

            foreach (KeyData key in data["relics"])
            {
                Relic relic = MainWindow.INSTANCE.RelicHandler.GetRelicFromName(key.KeyName);
                if (relic == null)
                {
                    try
                    {
                        int x = Convert.ToInt32(key.KeyName);
                    }
                    catch { continue; }
                    relic = MainWindow.INSTANCE.RelicHandler.GetRelicFromID(Int32.Parse(key.KeyName));
                }

                if (relic != null) relics.Add(relic);

            }


            return relics;
        }

        public void SaveBuild(string path)
        {
            StreamWriter sr = new StreamWriter(path);
            sr.Close();

            var parser = new FileIniDataParser();
            IniData data = parser.ReadFile(path);

            data.Sections.AddSection("build_info");
            data["build_info"].AddKey("name", Path.GetFileNameWithoutExtension(path));
            data["build_info"].AddKey("description", "This is a local build.");

            data.Sections.AddSection("relics");
            foreach (Relic relic in MainWindow.INSTANCE.SaveFileHandler.SelectedFile.Relics)
                data["relics"].AddKey(relic.ID.ToString(), "1");

            data.Sections.AddSection("inventory_list");
            for (int i = 0; i < MainWindow.INSTANCE.SaveFileHandler.SelectedFile.Inventory.InventoryItems.Count; i++)
                data["inventory_list"].AddKey($"inventory_list{i}", MainWindow.INSTANCE.SaveFileHandler.SelectedFile.Inventory.InventoryItems[i].GetItemString());

            data.Sections.AddSection("inventory");
            foreach (EquipmentSlot slot in MainWindow.INSTANCE.SaveFileHandler.SelectedFile.Inventory.Equipment.GetEquipmentList())
                data["inventory"].AddKey($"inventory{slot.Slot.ID}", slot.Item.GetItemString());

            data.Sections.AddSection("hero_info");
            for (int i = 0; i < MainWindow.INSTANCE.SaveFileHandler.SelectedFile.HeroInfo.Class.Talents.TalentList.Count; i++)
                data["hero_info"].AddKey($"talent_{MainWindow.INSTANCE.SaveFileHandler.SelectedFile.HeroInfo.Class.Talents.TalentList[i].ID}", MainWindow.INSTANCE.SaveFileHandler.SelectedFile.HeroInfo.Class.Talents.TalentList[i].Points.ToString());

            foreach (ActiveTalent activeTalent in MainWindow.INSTANCE.SaveFileHandler.SelectedFile.HeroInfo.ActiveTalents)
                data["hero_info"].AddKey($"active_talent_{activeTalent.ID}", activeTalent.Talent == null ? "0" : $"{MainWindow.INSTANCE.SaveFileHandler.SelectedFile.HeroInfo.Class.Talents.GetActiveTalents().IndexOf(activeTalent.Talent) + 1}");

            foreach (HeroTalent talent in MainWindow.INSTANCE.SaveFileHandler.SelectedFile.HeroInfo.GetFilteredHeroTalents())
                data["hero_info"].AddKey($"hero_talent_{talent.ID}", Convert.ToString(talent.SubID + 1));

            data["hero_info"].AddKey("class", MainWindow.INSTANCE.SaveFileHandler.SelectedFile.HeroInfo.Class.ID.ToString());
            data["hero_info"].AddKey("level", MainWindow.INSTANCE.SaveFileHandler.SelectedFile.HeroInfo.Level.ToString());
            data["hero_info"].AddKey("hero_level", MainWindow.INSTANCE.SaveFileHandler.SelectedFile.HeroInfo.HeroLevel.ToString());

            data.Sections.AddSection("mercenary");
            data["mercenary"].AddKey("type", MainWindow.INSTANCE.SaveFileHandler.SelectedFile.Mercenaries.GetSelected().ToString());

            foreach (MercenaryTalent talent in MainWindow.INSTANCE.SaveFileHandler.SelectedFile.Mercenaries.Talents.AllTalents)
                if (talent.MercenaryID == MainWindow.INSTANCE.SaveFileHandler.SelectedFile.Mercenaries.GetSelected())
                {
                    string x = talent.Type == "Offensive" ? "off" : "def";
                    data["mercenary"].AddKey($"minion_talent_{x}{talent.ID}", talent.Points.ToString());
                }

            SectionData section;
            if (MainWindow.INSTANCE.SaveFileHandler.SelectedFile.Mercenaries.GetSelected() == 0) data.Sections.AddSection("minion_inventory_melee");
            else data.Sections.AddSection("minion_inventory_ranged");

            foreach (EquipmentSlot slot in MainWindow.INSTANCE.SaveFileHandler.SelectedFile.Mercenaries.GetSelectedMerc().Equipment.GetEquipmentList())
            {
                if (MainWindow.INSTANCE.SaveFileHandler.SelectedFile.Mercenaries.GetSelected() == 0)
                    data["minion_inventory_melee"].AddKey($"inventory{slot.Slot.ID}", slot.Item.GetItemString());
                else
                    data["minion_inventory_ranged"].AddKey($"inventory{slot.Slot.ID}", slot.Item.GetItemString());
            }

            IniData parsedINIDataToBeSaved = data;
            parser.WriteFile(path, parsedINIDataToBeSaved);
        }
    }
}
