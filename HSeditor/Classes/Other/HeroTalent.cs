using System;
using System.Collections.Generic;

namespace HSeditor.Classes.Other
{
    public class HeroTalent
    {
        public int ID { get; private set; }
        public int SubID { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public List<Stat> Stats { get; private set; }
        public string StatFormatted { get; private set; }
        public string Prefix { get; private set; }
        public int Level { get; private set; }
        public string Sprite { get; private set; }
        public bool Selected { get; set; }

        public HeroTalent(int ID, int SubID, string Name, string Description, List<Stat> Stats, int Level, bool Selected = false)
        {
            this.ID = ID;
            this.SubID = SubID;
            this.Name = Name;
            this.Description = Description;
            this.Stats = Stats;
            this.Level = Level;
            this.Selected = Selected;
            this.Sprite = Environment.CurrentDirectory + @$"\Sprites\Hero Talents\herotalent_{ID}_{SubID}.png";
        }

        public HeroTalent DeepCopy()
        {
            HeroTalent temp = (HeroTalent)this.MemberwiseClone();
            return temp;
        }
    }
}
