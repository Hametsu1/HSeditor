using System;
using System.Collections.Generic;

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
    }
}
