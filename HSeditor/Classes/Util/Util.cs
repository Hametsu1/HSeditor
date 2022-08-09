using System;
using System.Collections.Generic;
using System.Linq;

namespace HSeditor.Classes.Util
{
    public class Util
    {
        public static int FormatString(string row)
        {
            if (row == null) return 0;
            return Int32.Parse((row.Trim('"').Split('.'))[0]);
        }

        public static bool ContainsFavorite(List<Item> Items)
        {
            foreach (Item item in Items)
                if (item.Favorite)
                    return true;
            return false;
        }

        public static bool MercenaryComp(int id)
        {
            if (id == 0 || id == 1 || id == 3 || id == 7) return true;
            return false;
        }

        public static string MakeUpper(string str, int cut = 9)
        {
            if (str == null) return "";
            string tempstr = str.Remove(0, cut);
            string ability = "";
            for (int i = 0; i < tempstr.Length; i++)
            {
                if (Char.IsUpper(tempstr[i]))
                {
                    ability += " " + tempstr[i];
                    continue;
                }
                ability += i == 0 ? Char.ToUpper(tempstr[i]) : tempstr[i];
            }
            return ability;
        }

        public static List<Stat> GetStatsFromString(string s)
        {
            try
            {
                List<string> list = s.Split('|').ToList();
                List<Stat> statlist = new List<Stat>();
                foreach (string stat in list)
                {
                    if (stat == "null") continue;
                    string[] temp = stat.Split(" ");
                    string statstr = "";
                    if (temp.Length > 2)
                    {
                        for (int i = 1; i < temp.Length; i++)
                        {
                            statstr += temp[i] + " ";
                        }
                        statstr = statstr.Remove(statstr.Length - 1);
                    }
                    else
                        statstr = temp[1];

                    Stat Stat = MainWindow.INSTANCE.StatHandler.GetStat(statstr, temp[0].Contains("%") ? "%" : "flat");
                    if (Stat == null) continue;
                    statlist.Add(new Stat(Stat.Name, Stat.DebugName, Stat.WíkiName, Stat.Type, Stat.Multiplier, Stat.Priority, Stat.HasPriority, Convert.ToDouble(temp[0].Trim('+').Trim('%'))));
                }
                return statlist;
            }
            catch
            {
                return new List<Stat>();
            }
        }
    }
}
