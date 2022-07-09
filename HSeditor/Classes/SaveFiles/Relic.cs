using HSeditor.Classes;
using HSeditor.Classes.Other;
using HSeditor.Classes.Util;
using HSeditor.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace HSeditor.SaveFiles
{
    public class Relic
    {
        public int ID { get; private set; }
        public string Name { get; set; }
        public string Description { get; private set; }
        public RelicType Type { get; set; }
        public string Sprite { get; private set; }
        public string Border { get; private set; }
        public List<Stat> Stats { get; set; }

        public Relic(int ID, string Name, string Description, RelicType Type, List<Stat> Stats)
        {
            this.ID = ID;
            this.Name = Name;
            this.Description = Description;
            this.Type = Type;
            this.Border = this.Type.Name == "Active" ? "#b39239" : "#FF483D85";
            this.Sprite = File.Exists(Environment.CurrentDirectory + @$"\Sprites\Relics\{this.Name}.png") ? Environment.CurrentDirectory + @$"\Sprites\Relics\{this.Name}.png" : null;
            this.Stats = Stats;
        }

        public bool ContainsStat(Stat stat)
        {
            foreach (Stat Stat in this.Stats)
                if (Stat.Name == stat.Name)
                    return true;
            return false;
        }
    }

    public class RelicHandler
    {
        public List<Relic> Relics { get; private set; }
        public List<RelicType> RelicTypes { get; private set; }
        public Relic RelicFilter { get; private set; }

        public RelicHandler()
        {
            this.RelicTypes = this.GetRelicTypes();
            this.RelicTypes.Insert(0, new RelicType(-1, "All Types", "#F6F6F6"));
            this.Relics = this.GetRelics();
            this.RelicFilter = new Relic(-1, "", "", this.RelicTypes[0], new List<Stat>());
        }

        public List<Relic> GetFilteredList()
        {
            List<Relic> filtered = new List<Relic>();

            foreach (Relic relic in this.Relics)
            {
                if (MainWindow.INSTANCE.SaveFileHandler != null && MainWindow.INSTANCE.SaveFileHandler.SelectedFile != null)
                {
                    if (MainWindow.INSTANCE.SaveFileHandler.SelectedFile.Relics.Contains(relic)) continue;
                    if (relic.Type.Name == "Active" && MainWindow.INSTANCE.SaveFileHandler.SelectedFile.GetActiveRelic() != null) continue;
                }

                if (!relic.Name.ToLower().Contains(this.RelicFilter.Name.ToLower()) && !relic.Description.ToLower().Contains(this.RelicFilter.Name.ToLower()) && this.RelicFilter.Name != "Search..." && this.RelicFilter.Name != "")
                    continue;

                if (relic.Type != this.RelicFilter.Type && this.RelicFilter.Type.ID != -1)
                    continue;

                if (relic.Stats.Count == 0 || !relic.ContainsStat(this.RelicFilter.Stats[0]))
                    if (this.RelicFilter.Stats[0].DebugName != "Default")
                        continue;


                filtered.Add(relic);
            }
            return filtered.OrderBy(o => o.Name).ToList();
        }


        public void LoadTemplate(string file)
        {
            try
            {
                var parser = new FileIniDataParser();
                IniData data = parser.ReadFile(file);

                ObservableCollection<Relic> relics = new ObservableCollection<Relic>();
                foreach (KeyData key in data["relics"])
                {
                    Relic relic = this.GetRelicFromID(Convert.ToInt32(key.Value));
                    if (relic != null)
                        relics.Add(relic);
                }
                MainWindow.INSTANCE.SaveFileHandler.SelectedFile.Relics = relics;
            }
            catch
            {
                MessageBox mb = new MessageBox("Template couldn't be loaded!", "There was an error trying to load the selected template.", "OK");
                mb.ShowDialog();
            }
        }

        public void SaveTemplate(string file)
        {
            var myfile = File.Create(file);
            myfile.Close();

            var parser = new FileIniDataParser();
            IniData data = parser.ReadFile(file);

            data.Sections.AddSection("relics");
            int i = 0;
            foreach (Relic relic in MainWindow.INSTANCE.SaveFileHandler.SelectedFile.Relics)
            {
                data["relics"].AddKey($"relic{i}", Convert.ToString(relic.ID));
                i++;
            }

            IniData parsedINIDataToBeSaved = data;
            parser.WriteFile(file, parsedINIDataToBeSaved);
        }

        private List<RelicType> GetRelicTypes()
        {
            List<RelicType> list = new List<RelicType>();
            var result = MainWindow.INSTANCE.iniDB.Read("SELECT * FROM RelicTypes, Colors WHERE RelicTypes.colorid = Colors.id");

            while (result.Read())
            {
                list.Add(new RelicType(result.GetInt32("id"), result.GetString("name"), result.GetString("hexcolor")));
            }
            result.Close();

            return list;
        }

        private RelicType GetRelicTypeFromString(string type)
        {
            foreach (RelicType relicType in this.RelicTypes)
                if (type.ToLower() == relicType.Name.ToLower())
                    return relicType;
            return null;
        }

        public string GetColorFromType(string Type)
        {
            switch (Type.ToLower())
            {
                case "active":
                    return "#2d5491"; //Blue
                case "orbitting":
                    return "#969641"; //Yellow
                case "statmod":
                    return "#5b8745"; //Green
                case "proc":
                    return "#4a8487"; //Cyan
                case "following":
                    return "#6c467d"; //Purple
            }
            return "#FFFFFF";
        }

        private List<Relic> GetRelics()
        {
            List<Relic> relics = new List<Relic>();

            var result = MainWindow.INSTANCE.iniDB.Read("SELECT * FROM Relics");

            while (result.Read())
            {
                relics.Add(new Relic(result.GetInt32("id"), result.GetString("name"), result.GetString("description"), this.GetRelicTypeFromString(result.GetString("type")), Util.GetStatsFromString(result.GetString("stats"))));
            }
            result.Close();

            return relics;
        }

        public Relic GetRelicFromID(int id)
        {
            foreach (Relic relic in this.Relics)
                if (relic.ID == id) return relic;

            return null;
        }

        public Relic GetRelicFromName(string name)
        {
            foreach (Relic relic in this.Relics)
                if (relic.Name == name) return relic;

            return null;
        }
    }
}
