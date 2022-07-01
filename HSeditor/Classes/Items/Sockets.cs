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
        public List<Rune> Runes { get; set; }
        public string RuneString { get; set; }

        public Sockets(List<Rune> runes)
        {
            Rune1 = MainWindow.INSTANCE.RuneHandler.Runes[0];
            Rune2 = MainWindow.INSTANCE.RuneHandler.Runes[0];
            Rune3 = MainWindow.INSTANCE.RuneHandler.Runes[0];
            Rune4 = MainWindow.INSTANCE.RuneHandler.Runes[0];
            Rune5 = MainWindow.INSTANCE.RuneHandler.Runes[0];
            Rune6 = MainWindow.INSTANCE.RuneHandler.Runes[0];

            if (runes != null)
            {
                if (runes.Count >= 1)
                    Rune1 = runes[0];
                if (runes.Count >= 2)
                    Rune2 = runes[1];
                if (runes.Count >= 3)
                    Rune3 = runes[2];
                if (runes.Count >= 4)
                    Rune4 = runes[3];
                if (runes.Count >= 5)
                    Rune5 = runes[4];
                if (runes.Count == 6)
                    Rune6 = runes[5];
            }

            Runes = this.GetRuneList();
            RuneString = this.GetRuneString();
        }

        public bool Compare(List<Rune> Runes)
        {
            List<Rune> runes = this.GetRuneList();
            if (runes.Count != Runes.Count) return false;
            for (int i = 0; i < runes.Count; i++)
            {
                if (runes[i].IngameID != Runes[i].IngameID) return false;
            }
            return true;
        }

        public string GetRuneString()
        {
            string s = "";
            if (Rune1.Name != "None")
                s += Rune1.Name + " ";
            if (Rune2.Name != "None")
                s += Rune2.Name + " ";
            if (Rune3.Name != "None")
                s += Rune3.Name + " ";
            if (Rune4.Name != "None")
                s += Rune4.Name + " ";
            if (Rune5.Name != "None")
                s += Rune5.Name + " ";
            if (Rune6.Name != "None")
                s += Rune6.Name;
            return s;
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
