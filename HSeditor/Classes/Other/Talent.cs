using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;

namespace HSeditor.Classes.Other
{
    public class Talent
    {
        public int ID { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Class { get; private set; }
        public int Level { get; private set; }
        public string Type { get; private set; }
        public int Points { get; set; }
        public string Sprite { get; private set; }
        public string Border { get; set; }
        public string HighlightBorder { get; set; }
        public int Row { get; private set; }
        public int Column { get; private set; }
        public string Tree { get; set; }

        public Talent(int ID, string Name, string Description, string Class, int Level, string Type, string Tree, int Row, int Column)
        {
            this.Points = 0;
            this.ID = ID;
            this.Name = Name;
            this.Description = Description;
            this.Class = Class;
            this.Level = Level;
            this.Type = Type;
            this.Sprite = Environment.CurrentDirectory + @$"\Sprites\Talents\{Class.ToLower()}_{this.ID}.png";
            this.Border = this.Type == "Active" ? "#b39239" : "#6e6b6a";
            this.HighlightBorder = this.Type == "Active" ? "#73602e" : "#474646";
            this.Tree = Tree;
            this.Row = Row;
            this.Column = Column;
        }

        public Talent DeepCopy()
        {
            Talent temp = (Talent)this.MemberwiseClone();
            return temp;
        }
    }

    public class TalentHandler
    {
        public List<Talent> Talents { get; private set; }
        public List<HeroTalent> HeroTalents { get; private set; }

        public TalentHandler()
        {
            this.Talents = this.GetTalents().OrderBy(o => o.Class).ThenBy(o => o.ID).ToList();
            this.HeroTalents = this.GetHeroTalents();
        }

        private List<HeroTalent> GetHeroTalents()
        {
            List<HeroTalent> list = new List<HeroTalent>();

            var result = MainWindow.INSTANCE.iniDB.Read("SELECT * FROM HeroTalents");

            while (result.Read())
            {
                list.Add(new HeroTalent(result.GetInt32("id"), result.GetInt32("subid"), result.GetString("name"), result.GetString("description"), GetStats(result.GetString("stats")), result.GetInt32("level")));
            }


            return list;
        }

        private List<Stat> GetStats(string stats)
        {
            List<Stat> statlist = new List<Stat>();
            string[] temp = stats.Split('|');
            foreach (string s in temp)
            {
                string type = s.Contains("%") ? "%" : "flat";
                int value = Convert.ToInt32(s.Split()[0].Trim('%').Trim('+'));
                string name = "";
                for (int i = 1; i < s.Split().Length; i++)
                    name += s.Split()[i] + " ";
                name = name.Remove(name.Length - 1);
                statlist.Add(new Stat(name, name, name, type, "0", 0, false, value));
            }
            return statlist;
        }

        public List<ActiveTalent> GetEmptyActiveTalents()
        {
            List<ActiveTalent> activeTalents = new List<ActiveTalent>();
            for (int i = 0; i < 4; i++)
                activeTalents.Add(new ActiveTalent(i, null));
            return activeTalents;
        }

        public List<HeroTalent> GetHeroTalentList()
        {
            List<HeroTalent> list = new List<HeroTalent>();
            foreach (HeroTalent heroTalent in this.HeroTalents)
                list.Add(heroTalent.DeepCopy());
            return list;
        }



        private List<Talent> GetTalents()
        {
            List<Talent> list = new List<Talent>();

            var result = MainWindow.INSTANCE.iniDB.Read("SELECT * FROM Talents");

            while (result.Read())
            {
                int row = 0;
                row = result.GetInt32("level") == 1 ? 0 : result.GetInt32("level") / 12;
                list.Add(new Talent(result.GetInt32("id"), result.GetString("name"), result.GetString("description"), result.GetString("class"), result.GetInt32("level"), result.GetString("type"), result.GetString("tree"), row, result.GetInt32("column")));
            }

            return list;
        }

        public BindingList<Talent> GetTalentsFromClass(string Class)
        {
            BindingList<Talent> list = new BindingList<Talent>();

            foreach (Talent talent in MainWindow.INSTANCE.ClassHandler.GetClassFromName(Class).Talents.TalentList)
                list.Add(talent.DeepCopy());

            return list;
        }

        public List<Talent> GetClassTalents(string Class)
        {
            List<Talent> list = new List<Talent>();

            foreach (Talent talent in this.Talents)
            {
                if (talent.Class == Class)
                    list.Add(talent.DeepCopy());

            }

            return list;
        }
    }
}
