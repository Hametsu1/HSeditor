using HSeditor.Classes.Other;
using System.Collections.Generic;

namespace HSeditor.Classes.SaveFiles
{
    public class HeroInfo
    {
        public Class? Class { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public int HeroLevel { get; set; }
        public int WormholeLevel { get; set; }
        public int ChaosTower { get; set; }
        public bool Hardcore { get; set; }
        public List<HeroTalent> HeroTalents { get; set; }
        public List<ActiveTalent> ActiveTalents { get; set; }

        public HeroInfo(Class Class, string Name, int Level, int HeroLevel, int WormholeLevel, int ChaosTower, bool Hardcore, List<HeroTalent> HeroTalents, List<ActiveTalent> ActiveTalents)
        {
            this.Class = Class;
            this.Name = Name;
            this.Level = Level;
            this.HeroLevel = HeroLevel;
            this.WormholeLevel = WormholeLevel;
            this.ChaosTower = ChaosTower;
            this.Hardcore = Hardcore;
            this.HeroTalents = HeroTalents;
            this.ActiveTalents = ActiveTalents;
        }

        // Build File
        public HeroInfo(Class Class, int Level, int HeroLevel, List<HeroTalent> HeroTalents, List<ActiveTalent> ActiveTalents)
        {
            this.Class = Class;
            this.Level = Level;
            this.HeroLevel = HeroLevel;
            this.HeroTalents = HeroTalents;
            this.ActiveTalents = ActiveTalents;
        }

        public HeroInfo DeepCopy()
        {
            HeroInfo temp = (HeroInfo)this.MemberwiseClone();
            temp.HeroTalents = MainWindow.INSTANCE.TalentHandler.GetHeroTalentList();
            temp.Class = this.Class.DeepCopy();

            return temp;
        }

        public List<HeroTalent> GetFilteredHeroTalents()
        {
            List<HeroTalent> list = new List<HeroTalent>();

            foreach (HeroTalent h in this.HeroTalents)
                if (h.Selected) list.Add(h);

            return list;
        }

        public int GetTalentPointsSpent()
        {
            int points = 0;
            foreach (Talent talent in this.Class.Talents.TalentList)
                points += talent.Points;

            return points;
        }

        public void ClearActiveTalents()
        {
            foreach (ActiveTalent talent in this.ActiveTalents)
                talent.Talent = null;
        }
    }
}
