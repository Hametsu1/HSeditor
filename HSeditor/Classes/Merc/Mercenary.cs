using HSeditor.Classes.Items;
using HSeditor.Classes.Other;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace HSeditor.Classes.Merc
{
    public class Mercenaries
    {
        public Mercenary Melee { get; private set; }
        public Mercenary Ranged { get; private set; }
        public MercenaryTalents Talents { get; set; }

        public Mercenaries(Mercenary Melee, Mercenary Ranged, List<MercenaryTalent> Talents)
        {
            this.Melee = Melee;
            this.Ranged = Ranged;
            this.Talents = new MercenaryTalents(Talents);
        }

        public int GetSelected()
        {
            if (this.Melee.Selected)
                return 0;

            return 1;
        }

        public Mercenary GetSelectedMerc()
        {
            if (this.Melee.Selected)
                return this.Melee;

            return this.Ranged;
        }

        public void SetSelected(int id)
        {
            if (id == 0) { this.Melee.Selected = true; this.Ranged.Selected = false; }
            else if (id == 1) { this.Melee.Selected = false; this.Ranged.Selected = true; }
        }

        public Mercenaries DeepCopy(bool equipment = true)
        {
            Mercenaries temp = (Mercenaries)this.MemberwiseClone();
            Equipment melee = new Equipment();
            melee.SetToMercenary();
            Equipment ranged = new Equipment();
            ranged.SetToMercenary();
            foreach (EquipmentSlot eq in Melee.Equipment.GetEquipmentList())
                melee.EquipItem(eq.Item.DeepCopy(), eq.Slot.ID, false);
            temp.Melee = new Mercenary(Melee.Name, Melee.Type, melee, Melee.Selected);
            foreach (EquipmentSlot eq in Ranged.Equipment.GetEquipmentList())
                ranged.EquipItem(eq.Item.DeepCopy(), eq.Slot.ID, false);
            temp.Ranged = new Mercenary(Ranged.Name, Ranged.Type, ranged, Ranged.Selected);
            temp.Talents = new MercenaryTalents(temp.Talents.AllTalents);
            return temp;
        }
    }
    public class Mercenary
    {
        public string Name { get; private set; }
        public MercenaryType Type { get; private set; }
        public Equipment Equipment { get; private set; }
        public bool Selected { get; set; }

        public Mercenary(string Name, MercenaryType Type, Equipment equipment, bool Selected)
        {
            this.Name = Name;
            this.Type = Type;
            this.Equipment = equipment;
            this.Selected = Selected;
        }
    }

    public class MercenaryTalent
    {
        public int ID { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public int Level { get; private set; }
        public string Type { get; private set; }
        public int Points { get; set; }
        public string Sprite { get; private set; }
        public int Row { get; private set; }
        public int Column { get; private set; }
        public int MercenaryID { get; private set; }

        public MercenaryTalent(int ID, string Name, string Description, int Level, int MercenaryID, string Type, int Column, int Points = 0)
        {
            this.ID = ID;
            this.Name = Name;
            this.Description = Description;
            this.Type = Type;
            this.Points = Points;
            this.Level = Level;
            this.MercenaryID = MercenaryID;
            this.Column = Column;
            this.Row = this.Level == 1 ? 0 : this.Level / 12;
            string x = this.Type == "Offensive" ? "off" : "def";
            if (x == "def")
                this.Sprite = Environment.CurrentDirectory + @$"\Sprites\Mercenary Talents\{x}{this.ID}.png";
            else
                this.Sprite = Environment.CurrentDirectory + @$"\Sprites\Mercenary Talents\{x}{this.ID}_{this.MercenaryID}.png";
        }
    }

    public class MercenaryType
    {
        public int ID { get; private set; }
        public string Name { get; private set; }

        public MercenaryType(int ID, string Name)
        {
            this.ID = ID;
            this.Name = Name;
        }
    }

    public class MercenaryHandler
    {
        public List<MercenaryType> Types { get; private set; }
        public List<MercenaryTalent> Talents { get; private set; }

        public MercenaryHandler()
        {
            this.Types = this.GetMercenaryTypes();
            this.Talents = this.GetMercenaryTalents();
        }

        public List<MercenaryTalent> GetDefaultTalents()
        {
            List<MercenaryTalent> talents = new List<MercenaryTalent>();
            foreach (MercenaryTalent talent in this.Talents)
                talents.Add(new MercenaryTalent(talent.ID, talent.Name, talent.Description, talent.Level, talent.MercenaryID, talent.Type, talent.Column, 0));
            return talents;
        }

        private List<MercenaryTalent> GetMercenaryTalents()
        {
            List<MercenaryTalent> list = new List<MercenaryTalent>();

            var result = MainWindow.INSTANCE.iniDB.Read("SELECT * FROM MercenaryTalents");

            while (result.Read())
            {
                list.Add(new MercenaryTalent(result.GetInt32("id"), result.GetString("name"), result.GetString("description"), result.GetInt32("level"), result.GetInt32("mercenaryid"), result.GetString("type"), result.GetInt32("column")));
            }
            return list;
        }

        private List<MercenaryType> GetMercenaryTypes()
        {
            List<MercenaryType> list = new List<MercenaryType>();

            var result = MainWindow.INSTANCE.iniDB.Read("SELECT * FROM MercenaryTypes");

            while (result.Read())
            {
                list.Add(new MercenaryType(result.GetInt32("id"), result.GetString("name")));
            }
            return list;
        }

        public MercenaryType GetMercenaryTypeFromID(int ID)
        {
            foreach (MercenaryType mercenaryType in this.Types)
                if (mercenaryType.ID == ID)
                    return mercenaryType;
            return null;
        }

        public MercenaryType GetMercenaryTypeFromName(string name)
        {
            foreach (MercenaryType type in this.Types)
                if (type.Name == name)
                    return type;
            return null;
        }

        public List<MercenaryTalent> GetMercenaryTalentsFromString(string name)
        {
            List<MercenaryTalent> list = new List<MercenaryTalent>();
            string type = name.Contains("def") ? "Defensive" : "Offensive";
            int id = (int)Char.GetNumericValue(name[name.Length - 1]);
            foreach (MercenaryTalent talent in this.Talents)
                if (talent.ID == id && talent.Type == type)
                    list.Add(talent);
            return list;
        }
    }
}
