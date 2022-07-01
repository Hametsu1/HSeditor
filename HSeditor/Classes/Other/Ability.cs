using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace HSeditor
{
    public class Ability
    {
        public string Name { get; private set; }
        public int ID { get; private set; }

        public Ability(string Name, int ID)
        {
            this.Name = Name;
            this.ID = ID;
        }
    }

    public class AbilityHandler
    {
        public List<Ability> Abilities { get; private set; }
        public Ability None { get; private set; }

        public AbilityHandler()
        {
            this.Abilities = this.GetAbilities().OrderBy(o => o.Name).ToList();
            this.Abilities.Insert(0, new Ability("None", 0));
            this.None = this.Abilities[0];
        }

        private List<Ability> GetAbilities()
        {
            List<Ability> abilities = new List<Ability>();

            var result = MainWindow.INSTANCE.iniDB.Read("SELECT * FROM Abilities");

            while (result.Read())
            {
                abilities.Add(new Ability(result.GetString("name"), result.GetInt32("id")));
            }
            result.Close();
            return abilities;
        }

        public Ability GetAbilityFromName(string name)
        {
            if (name == null) return null;
            foreach (Ability ability in this.Abilities)
                if (ability.Name.Trim(' ').ToLower() == name.ToLower())
                    return ability;

            return null;
        }

        public Ability GetAbilityFromID(int id)
        {
            foreach (Ability ability in this.Abilities)
                if (ability.ID == id) return ability;

            return null;
        }
    }
}
