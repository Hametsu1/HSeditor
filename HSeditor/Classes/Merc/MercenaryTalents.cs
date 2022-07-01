using System.Collections.Generic;

namespace HSeditor.Classes.Merc
{
    public class MercenaryTalents
    {
        public List<MercenaryTalent> AllTalents { get; private set; }

        public MercenaryTalents(List<MercenaryTalent> Talents = null)
        {
            this.AllTalents = MainWindow.INSTANCE.MercenaryHandler.GetDefaultTalents();
            foreach (MercenaryTalent talent in Talents)
            {
                foreach (MercenaryTalent talent2 in AllTalents)
                    if (talent2.Name == talent.Name)
                    {
                        talent2.Points = talent.Points;
                        break;
                    }
            }
        }

        public List<MercenaryTalent> GetOffensiveTalents(int id)
        {
            List<MercenaryTalent> list = new List<MercenaryTalent>();
            foreach (MercenaryTalent mercenaryTalent in this.AllTalents)
                if (mercenaryTalent.Type == "Offensive" && mercenaryTalent.MercenaryID == id)
                    list.Add(mercenaryTalent);
            return list;
        }

        public List<MercenaryTalent> GetOffensiveList(int id)
        {
            List<MercenaryTalent> list = new List<MercenaryTalent>();
            foreach (MercenaryTalent mercenaryTalent in this.AllTalents)
                if (mercenaryTalent.Type == "Offensive" && mercenaryTalent.MercenaryID == id && mercenaryTalent.Points > 0)
                    list.Add(mercenaryTalent);
            return list;
        }

        public List<MercenaryTalent> GetDefensiveList(int id)
        {
            List<MercenaryTalent> list = new List<MercenaryTalent>();
            foreach (MercenaryTalent mercenaryTalent in this.AllTalents)
                if (mercenaryTalent.Type == "Defensive" && mercenaryTalent.MercenaryID == id && mercenaryTalent.Points > 0)
                    list.Add(mercenaryTalent);
            return list;
        }

        public List<MercenaryTalent> GetDefensiveTalents()
        {
            List<MercenaryTalent> list = new List<MercenaryTalent>();
            foreach (MercenaryTalent mercenaryTalent in this.AllTalents)
                if (mercenaryTalent.Type == "Defensive")
                    list.Add(mercenaryTalent);
            return list;
        }

        public int GetPointsSpent()
        {
            int pointsSpent = 0;
            foreach (MercenaryTalent mercenaryTalent in this.AllTalents)
                if (mercenaryTalent.MercenaryID == MainWindow.INSTANCE.SaveFileHandler.SelectedFile.Mercenaries.GetSelected())
                    pointsSpent += mercenaryTalent.Points;
            return pointsSpent;
        }
    }
}
