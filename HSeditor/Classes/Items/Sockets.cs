using System.Collections.Generic;

namespace HSeditor.Classes.Items
{
    public class Sockets
    {
        public Rune Rune1 { get; set; }
        public Rune Rune2 { get; set; }
        public Rune Rune3 { get; set; }
        public Rune Rune4 { get; set; }
        public Rune Rune5 { get; set; }
        public Rune Rune6 { get; set; }
        public List<Rune> Runes { get { return this.GetRuneList(); } }
        public string RuneString { get { return this.GetRuneString(); } }

        public Sockets(List<Rune> runes)
        {
            Rune1 = new Rune(0, "None");
            Rune2 = new Rune(0, "None");
            Rune3 = new Rune(0, "None");
            Rune4 = new Rune(0, "None");
            Rune5 = new Rune(0, "None");
            Rune6 = new Rune(0, "None");

            if (runes != null)
            {
                if (runes.Count >= 1 && runes[0] != null)
                    Rune1 = runes[0];
                if (runes.Count >= 2 && runes[1] != null)
                    Rune2 = runes[1];
                if (runes.Count >= 3 && runes[2] != null)
                    Rune3 = runes[2];
                if (runes.Count >= 4 && runes[3] != null)
                    Rune4 = runes[3];
                if (runes.Count >= 5 && runes[4] != null)
                    Rune5 = runes[4];
                if (runes.Count == 6 && runes[5] != null)
                    Rune6 = runes[5];
            }
        }

        public void SetRune(int id, Rune rune)
        {
            switch (id)
            {
                case 0:
                    Rune1 = rune;
                    break;
                case 1:
                    Rune2 = rune;
                    break;
                case 2:
                    Rune3 = rune;
                    break;
                case 3:
                    Rune4 = rune;
                    break;
                case 4:
                    Rune5 = rune;
                    break;
                case 5:
                    Rune6 = rune;
                    break;
            }
        }

        /*
        public bool Compare(List<Rune> Runes)
        {
            List<Rune> runes = this.GetRuneList();
            if (runes.Count != Runes.Count) return false;
            for (int i = 0; i < runes.Count; i++)
            {
                if (runes[i].IngameID != Runes[i].IngameID) return false;
            }
            return true;
        }*/

        public string GetRuneString()
        {
            string s = "";
            if (Rune1.Name != "None")
                s += Rune1.Name + ", ";
            if (Rune2.Name != "None")
                s += Rune2.Name + ", ";
            if (Rune3.Name != "None")
                s += Rune3.Name + ", ";
            if (Rune4.Name != "None")
                s += Rune4.Name + ", ";
            if (Rune5.Name != "None")
                s += Rune5.Name + ", ";
            if (Rune6.Name != "None")
                s += Rune6.Name;

            return s.Trim(' ').Trim(',');
        }

        public List<Rune> GetRuneList()
        {
            List<Rune> list = new List<Rune>();
            list.Add(Rune1);
            list.Add(Rune2);
            list.Add(Rune3);
            list.Add(Rune4);
            list.Add(Rune5);
            list.Add(Rune6);
            return list;
        }
    }
}
