using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace HSeditor.Classes.Other
{
    public class WeaponType
    {
        public string Name { get; private set; }
        public int ID { get; private set; }
        public string Sprite { get; private set; }
        public int DefaultHandedType { get; private set; }

        public WeaponType(string Name, int ID, int DefaultHandedType)
        {
            this.Name = Name;
            this.ID = ID;
            this.DefaultHandedType = DefaultHandedType;
            if (this.Name == "None") return;
            this.Sprite = @"pack://application:,,,/HSeditor;component/Resources/" + Name + ".png";
        }
    }

    public class WeaponTypeHandler
    {
        public List<WeaponType> WeaponTypes { get; private set; }
        public List<WeaponType> WeaponTypesFiltered { get; private set; }

        public WeaponTypeHandler()
        {
            this.WeaponTypes = this.GetWeaponTypes();
            this.WeaponTypesFiltered = new List<WeaponType>(WeaponTypes);
            this.WeaponTypesFiltered.Remove(this.WeaponTypesFiltered.Find(o => o.ID == 0));
            this.WeaponTypesFiltered.Add(new WeaponType("All Weapons", -1, 1));
            this.WeaponTypesFiltered = this.WeaponTypesFiltered.OrderBy(o => o.ID).ToList();
        }

        private List<WeaponType> GetWeaponTypes()
        {
            List<WeaponType> weaponTypes = new List<WeaponType>();
            var result = MainWindow.INSTANCE.iniDB.Read("SELECT * FROM WeaponTypes");

            while (result.Read())
            {
                weaponTypes.Add(new WeaponType(result.GetString("name"), result.GetInt32("id"), result.GetInt32("defaultHanded")));
            }

            return weaponTypes;
        }

        public WeaponType GetWeaponTypeFromID(int id)
        {
            foreach (WeaponType weaponType in this.WeaponTypes)
                if (weaponType.ID == id)
                    return weaponType;

            return new WeaponType("Unknown Weapontype", id, 1);
        }

        public WeaponType GetWeaponTypeFromName(string name)
        {
            foreach (WeaponType weaponType in this.WeaponTypes)
                if (weaponType.Name == name)
                    return weaponType;

            return null;
        }
    }
}
