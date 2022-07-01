using HSeditor.Classes.Other;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace HSeditor
{
    public class Rune
    {
        public string Name { get; set; }
        public int EditorID { get; private set; }
        public int IngameID { get; private set; }
        public string Description { get; private set; }
        public RuneType Type { get; set; }
        public int Amount { get; set; }
        public string Sprite { get; private set; }

        public Rune(string Name, int ID, int IngameID, string Description, RuneType Type)
        {
            this.Name = Name;
            this.EditorID = ID;
            this.IngameID = IngameID;
            this.Description = Description;
            this.Type = Type;
            this.Sprite = Environment.CurrentDirectory + $@"\Sprites\Runes\{this.Name}_.png";
        }

        public Rune(int ID, int Amount)
        {
            this.IngameID = ID;
            this.Amount = Amount;

            Rune equivalent = MainWindow.INSTANCE.RuneHandler.GetRuneFromID(ID);
            if (equivalent == null) return;
            this.EditorID = equivalent.IngameID;
            this.Description = equivalent.Description;
            this.Name = equivalent.Name;
            this.Type = new RuneType(equivalent.Type.ID, equivalent.Type.Name, equivalent.Type.Color);
        }

        public Rune(string Name, RuneType Type)
        {
            this.Name = Name;
            this.Type = Type;
        }

        public Rune DeepCopy()
        {
            Rune temp = (Rune)this.MemberwiseClone();
            return temp;
        }
    }

    public class RuneHandler
    {
        public List<Rune> Runes { get; private set; }
        public List<RuneType> RuneTypesFiltered { get; private set; }
        public List<Rune> RunesFiltered { get; private set; }
        public List<RuneType> RuneTypes { get; private set; }

        public RuneHandler()
        {
            this.RuneTypesFiltered = this.GetRuneTypes();
            this.RuneTypes = new List<RuneType>(RuneTypesFiltered);
            this.RuneTypes.Add(new RuneType(-1, "All Types", "#F6F6F6"));
            this.RuneTypes = this.RuneTypes.OrderBy(o => o.ID).ToList();
            this.RuneTypesFiltered.Add(new RuneType(-1, "Null", "#F6F6F6"));
            this.Runes = this.GetRunes().OrderBy(o => o.Type.ID).ToList();
            this.RunesFiltered = new List<Rune>(this.Runes);
            this.Runes.Insert(0, new Rune("None", 0, 0, "Default", this.GetRuneTypeFromString("Null")));
        }

        private List<RuneType> GetRuneTypes()
        {
            List<RuneType> list = new List<RuneType>();

            var result = MainWindow.INSTANCE.iniDB.Read("SELECT * FROM RuneTypes, Colors WHERE RuneTypes.colorid == Colors.id");

            while (result.Read())
            {
                list.Add(new RuneType(result.GetInt32("id"), result.GetString("name"), result.GetString("hexcolor")));
            }

            return list;
        }

        private List<Rune> GetRunes()
        {
            List<Rune> runes = new List<Rune>();

            var result = MainWindow.INSTANCE.iniDB.Read("SELECT * FROM Runes");

            while (result.Read())
            {
                runes.Add(new Rune(result.GetString("name"), result.GetInt32("id"), result.GetInt32("ingameid"), result.GetString("description"), this.GetRuneTypeFromString(result.GetString("type"))));
            }
            result.Close();

            return runes;
        }

        private RuneType GetRuneTypeFromString(string type)
        {
            foreach (RuneType runeType in this.RuneTypesFiltered)
                if (runeType.Name == type)
                    return runeType;
            return null;
        }

        public Rune GetRuneFromID(int id)
        {
            foreach (Rune rune in this.Runes)
                if (rune.IngameID == id) return rune;

            return null;
        }

        public List<Rune> GetRunesFromSaveString(string savestring)
        {
            List<Rune> runes = new List<Rune>();
            List<string> runeids = savestring.Split('|').ToList();
            runeids.ForEach(o => { runes.Add(GetRuneFromID(Int32.Parse(o))); });
            return runes;
        }
    }
}
