using HSeditor.Classes.Other;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace HSeditor.Classes
{
    public class Class
    {
        public string Name { get; private set; }
        public int ID { get; private set; }
        public string Sprite { get; private set; }
        public Talents Talents { get; private set; }

        public Class(string Name, int ID, string SpritePath, List<Talent> talents = null)
        {
            this.Name = Name;
            this.ID = ID;
            if (ID == 99) return;
            this.Sprite = SpritePath;
            this.Talents = new Talents(this, talents);
        }

        public Class DeepCopy()
        {
            Class temp = (Class)this.MemberwiseClone();
            List<Talent> list = new List<Talent>();
            foreach (Talent talent in this.Talents.TalentList)
                list.Add(talent.DeepCopy());
            temp.Talents = new Talents(temp, list);
            return temp;
        }
    }


    public class ClassHandler
    {
        public List<Class> Classes { get; private set; }
        public List<Class> Classes_Filtered { get; private set; }
        public Class AllClasses { get; private set; }

        public ClassHandler()
        {
            this.Classes = this.GetClasses();
            this.AllClasses = this.Classes[0];
            this.Classes.Add(new Class("NewChar", 100, Environment.CurrentDirectory + @$"\Sprites\Classes\NewChar.png"));
            this.Classes_Filtered = new List<Class>(this.Classes);
            this.Classes_Filtered.RemoveAt(0);
            this.Classes_Filtered.Remove(this.Classes_Filtered.Last());
        }

        private List<Class> GetClasses()
        {
            List<Class> classes = new List<Class>();

            var result = MainWindow.INSTANCE.iniDB.Read("SELECT * FROM Classes");

            while (result.Read())
            {
                classes.Add(new Class(result.GetString("name"), result.GetInt32("ingameid"), Environment.CurrentDirectory + @$"\Sprites\Classes\{result.GetString("name")}.png"));
            }
            result.Close();
            return classes;
        }

        public Class GetClassFromName(string name)
        {
            foreach (Class c in this.Classes)
                if (Regex.Replace(c.Name.ToLower(), @"\s+", "") == Regex.Replace(name.ToLower(), @"\s+", "")) return c;

            return null;
        }

        public Class GetClassFromID(int classid)
        {
            foreach (Class Class in this.Classes)
                if (Class.ID == classid)
                    return Class;

            return null;
        }

    }
}

