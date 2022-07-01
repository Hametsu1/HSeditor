using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace HSeditor
{
    public class Rarity
    {
        public string Name { get; private set; }
        public int IngameID { get; private set; }
        public int EditorID { get; private set; }
        public string Color { get; private set; }
        public int MaxUpgradeLevel { get; private set; }

        public Rarity(string Name, int IngameID, int EditorID, string Color, int MaxUpgradeLevel)
        {
            this.Name = Name;
            this.IngameID = IngameID;
            this.EditorID = EditorID;
            this.Color = Color;
            this.MaxUpgradeLevel = MaxUpgradeLevel;
        }
    }

    public class RarityHandler
    {
        public List<Rarity> Raritities { get; private set; }
        public List<Rarity> RaritiesFiltered { get; private set; }

        public RarityHandler()
        {
            this.Raritities = this.GetRarities().OrderBy(o => o.EditorID).ToList();
            this.RaritiesFiltered = new List<Rarity>();
            this.Raritities.ForEach(o => { if (o.Color != "#000000") RaritiesFiltered.Add(o); });
        }

        private List<Rarity> GetRarities()
        {
            List<Rarity> rarities = new List<Rarity>();

            var result = MainWindow.INSTANCE.iniDB.Read("SELECT Rarities.id, Rarities.ingameid, Rarities.name,Rarities.maxupgrade, Colors.hexcolor FROM Rarities, Colors WHERE Rarities.colorid == Colors.id");

            while (result.Read())
            {
                rarities.Add(new Rarity(result.GetString("name"), result.GetInt32("ingameid"), result.GetInt32("id"), result.GetString("hexcolor"), result.GetInt32("maxupgrade")));
            }
            result.Close();
            return rarities;
        }

        public Rarity GetRarityFromID(int id)
        {
            foreach (Rarity r in this.Raritities)
                if (r.IngameID == id) return r;

            return null;
        }

        public Rarity GetRarityFromEditorID(int id)
        {
            foreach (Rarity r in this.Raritities)
                if (r.EditorID == id) return r;

            return null;
        }
    }
}
