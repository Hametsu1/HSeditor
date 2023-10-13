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
        public string BackgroundColor { get; private set; }
        public string BackgroundColor2 { get; private set; }
        public string TooltipBorderColor { get; private set; }
        public string TooltipNameColor { get; private set; }
        public bool ShowInTooltip { get; private set; }

        public Rarity(string Name, int IngameID, int EditorID, string Color, string backgroundColor, string backgroundColor2, object TooltipBorderColor = null, object TooltipNameColor = null, bool showInTooltip = false)
        {
            this.Name = Name;
            this.IngameID = IngameID;
            this.EditorID = EditorID;
            this.Color = Color;
            this.BackgroundColor = backgroundColor;
            this.BackgroundColor2 = backgroundColor2;
            this.TooltipBorderColor = TooltipBorderColor == null || TooltipBorderColor.GetType().Name == "DBNull" ? this.Color : (string)TooltipBorderColor;
            this.TooltipNameColor = TooltipNameColor == null || TooltipNameColor.GetType().Name == "DBNull" ? this.Color : (string)TooltipNameColor;
            this.ShowInTooltip = showInTooltip;
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
            this.Raritities.ForEach(o => { if (o.Color != "#000000" && o.EditorID < 20) RaritiesFiltered.Add(o); });
        }

        private List<Rarity> GetRarities()
        {
            List<Rarity> rarities = new List<Rarity>();

            var result = MainWindow.INSTANCE.iniDB.Read("SELECT Rarities.id,Rarities.name, Rarities.ingameid, Rarities.backgroundcolor, Rarities.backgroundcolor2,Rarities.tooltipborder,Rarities.tooltipname,Rarities.showrarityintooltip, Colors.hexcolor FROM Rarities, Colors WHERE Rarities.colorid == Colors.id");

            while (result.Read())
            {
                rarities.Add(new Rarity(result.GetString("name"), result.GetInt32("ingameid"), result.GetInt32("id"), result.GetString("hexcolor"), result.GetString("backgroundcolor"), result.GetString("backgroundcolor2"), result.GetValue("tooltipborder"), result.GetValue("tooltipname"), result.GetBoolean("showrarityintooltip")));
            }
            result.Close();
            return rarities;
        }

        public Rarity GetRarityFromID(int id)
        {
            foreach (Rarity r in this.Raritities)
                if (r.IngameID == id) return r;

            return new Rarity("Unknown Rarity", id, 9999, "#5b5c54", "#2e2e2e", "#1a1919", null, null, true);
        }

        public Rarity GetRarityFromEditorID(int id)
        {
            foreach (Rarity r in this.Raritities)
                if (r.EditorID == id) return r;

            return null;
        }
    }
}
