using System.Collections.Generic;
using System.Data;
using System.Text;

namespace HSeditor.Classes.Other
{
    public class Set
    {
        public int ID { get; private set; }
        public string Name { get; private set; }
        public string EffectString { get; private set; }
        public string EffectName { get; private set; }
        public string Description { get; private set; }
        public Class Class { get; private set; }

        public Set(int ID, string Name, string EffectName, string Description, Class Class)
        {
            this.ID = ID;
            this.Name = Name;
            this.EffectString = "[" + Name + "]";
            this.EffectName = EffectName;
            this.Description = Description;
            this.Class = Class;
        }
    }

    public class SetHandler
    {
        public List<Set> Sets { get; private set; }
        public Set None { get; private set; }

        public SetHandler()
        {
            this.Sets = this.GetSets();
            this.None = this.Sets[0];
        }

        private List<Set> GetSets()
        {
            List<Set> sets = new List<Set>();

            var result = MainWindow.INSTANCE.iniDB.Read("SELECT * FROM Sets");

            while (result.Read())
            {
                sets.Add(new Set(result.GetInt32("id"), result.GetString("name"), result.GetString("effectname"), result.GetString("description"), result.GetInt32("classid") == -1 ? MainWindow.INSTANCE.ClassHandler.AllClasses : MainWindow.INSTANCE.ClassHandler.GetClassFromID(result.GetInt32("classid"))));
            }
            result.Close();

            return sets;
        }

        public Set GetSetFromID(int id)
        {
            Set set = null;
            this.Sets.ForEach(o => { if (o.ID == id) set = o; });
            return set;
        }
    }
}
