using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSeditor.Classes.Other
{
    public class Augment
    {
        public int ID { get; private set; }
        public string Name { get; private set; }
        public Class Class { get; private set; }

        public Augment(int iD, string name, Class @class)
        {
            ID = iD;
            Name = name;
            Class = @class;
        }
    }

    public class AugmentHandler
    {
        public List<Augment> Augments { get; private set; }

        public AugmentHandler()
        {
            this.Augments = this.GetAugments();
        }

        public List<Augment> GetAugments()
        {
            List<Augment> augments = new List<Augment>();

            var result = MainWindow.INSTANCE.iniDB.Read("SELECT * FROM Augments");

            while (result.Read())
            {
                augments.Add(new Augment(result.GetInt32("id"), result.GetString("name"), MainWindow.INSTANCE.ClassHandler.GetClassFromID(result.GetInt32("classid"))));
            }

            return augments.OrderBy(o => o.ID).ToList();
        }

        public Augment GetAugmentFromID(int id)
        {
            Augment augment = this.Augments.Find(o => o.ID == id);
            return augment == null ? new Augment(id, "Unknown Augment", MainWindow.INSTANCE.ClassHandler.GetClassFromID(-1)) : augment;
        }
    }
}
