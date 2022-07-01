using HSeditor.Classes.Other;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace HSeditor
{
    public class Uber
    {
        public int ID { get; private set; }
        public string Name { get; set; }
        public UberType Type { get; set; }
        public int Amount { get; set; }
        public string Sprite { get; private set; }

        public Uber(int ID, string Name, UberType Type)
        {
            this.ID = ID;
            this.Name = Name;
            this.Type = Type;
            this.Sprite = File.Exists(Environment.CurrentDirectory + @$"\Sprites\Ubers\{Name}.png") ? Environment.CurrentDirectory + @$"\Sprites\Ubers\{Name}.png" : "";
        }

        public Uber(int ID, int Amount)
        {
            this.ID = ID;
            this.Amount = Amount;

            Uber equivalent = MainWindow.INSTANCE.UberHandler.GetEquivalent(this);
            this.Name = equivalent.Name;
            this.Type = equivalent.Type;
            this.Sprite = equivalent.Sprite;
        }

        public Uber(string Name, UberType Type)
        {
            this.Name = Name;
            this.Type = Type;
        }

        public Uber DeepCopy()
        {
            Uber temp = (Uber)this.MemberwiseClone();
            return temp;
        }
    }

    public class UberHandler
    {
        public List<UberType> UberTypesFiltered { get; private set; }
        public List<UberType> UberTypes { get; private set; }
        public List<Uber> Ubers { get; private set; }

        public UberHandler()
        {
            this.UberTypesFiltered = this.GetUberTypes();
            this.UberTypes = new List<UberType>(UberTypesFiltered).OrderBy(o => o.Name).ToList();
            this.UberTypes.Insert(0, new UberType(-1, "All Types", "#F6F6F6"));
            this.Ubers = this.GetUbers().OrderBy(o => o.Type.Name).ToList();
        }

        private UberType GetUberTypeFromString(string type)
        {
            foreach (UberType uberType in UberTypesFiltered)
                if (uberType.Name == type)
                    return uberType;
            return null;
        }


        private List<UberType> GetUberTypes()
        {
            List<UberType> uberTypes = new List<UberType>();
            var result = MainWindow.INSTANCE.iniDB.Read("SELECT * FROM UberTypes, Colors WHERE UberTypes.colorid == Colors.id");

            while (result.Read())
            {
                uberTypes.Add(new UberType(result.GetInt32("id"), result.GetString("name"), result.GetString("hexcolor")));
            }
            return uberTypes;
        }

        private List<Uber> GetUbers()
        {
            List<Uber> ubers = new List<Uber>();
            var result = MainWindow.INSTANCE.iniDB.Read("SELECT * FROM Ubers");

            while (result.Read())
            {
                ubers.Add(new Uber(result.GetInt32("id"), result.GetString("name"), this.GetUberTypeFromString(result.GetString("type"))));
            }
            result.Close();

            return ubers;
        }

        public Uber GetEquivalent(Uber uber)
        {
            foreach (Uber uber2 in this.Ubers)
            {
                if (uber.ID == uber2.ID)
                    return uber2;
            }
            return null;
        }
    }
}
