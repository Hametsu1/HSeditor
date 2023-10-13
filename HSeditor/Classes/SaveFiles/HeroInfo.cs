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

        public HeroInfo(Class Class, string Name, int Level, int HeroLevel, int WormholeLevel, int ChaosTower, bool Hardcore)
        {
            this.Class = Class;
            this.Name = Name;
            this.Level = Level;
            this.HeroLevel = HeroLevel;
            this.WormholeLevel = WormholeLevel;
            this.ChaosTower = ChaosTower;
            this.Hardcore = Hardcore;
        }

        // Build File
        public HeroInfo(Class Class, int Level, int HeroLevel)
        {
            this.Class = Class;
            this.Level = Level;
            this.HeroLevel = HeroLevel;
        }

        public HeroInfo DeepCopy()
        {
            HeroInfo temp = (HeroInfo)this.MemberwiseClone();
            temp.Class = this.Class.DeepCopy();

            return temp;
        }
    }
}
