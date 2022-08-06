using HSeditor.Classes.Other;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSeditor.Classes.Util
{
    public static class Export
    {

        public static void Stats()
        {
            string filename = "";
            StreamWriter sr = null;
            if (Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @$"\Hero_Siege\hseditor\export")) Directory.Delete(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @$"\Hero_Siege\hseditor\export", true);
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @$"\Hero_Siege\hseditor\export");
            StreamWriter sr2 = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @$"\Hero_Siege\hseditor\export\masspages.txt");
            List<string> Satanics = new List<string>();
            List<string> Set = new List<string>();
            List<string> Heroic = new List<string>();
            List<string> Angelics = new List<string>();
            foreach (Item item in MainWindow.INSTANCE.ItemHandler.AllItems.OrderBy(o => o.Slot.ID).ThenBy(o => o.WeaponType.ID).ThenBy(o => o.Rarity.Name == "Angelic").ThenBy(o => o.Rarity.EditorID).ToList())
            {
                if (item.Rarity.Name == "Runeword" || item.Rarity.Name == "Heroic Set" || item.Stats == null) continue;
                string file = item.Slot.ID == 3 ? item.WeaponType.Name : item.Slot.Name;
                if (file != filename)
                { if (filename != "") { WriteMassPages(Satanics, Set, Heroic, Angelics, sr2, filename); Satanics.Clear(); Set.Clear(); Heroic.Clear(); Angelics.Clear(); } filename = file; if (sr != null) sr.Close(); sr = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @$"\Hero_Siege\hseditor\export\{file}.txt"); };
                switch (item.Rarity.Name) { case "Satanic": Satanics.Add("{{:" + item.Name + "}}"); break; case "Set": Set.Add("{{:" + item.Name + "}}"); break; case "Heroic": Heroic.Add("{{:" + item.Name + "}}"); break; case "Angelic": Angelics.Add("{{:" + item.Name + "}}"); break; }
                sr.WriteLine($"<!--{item.Name}-->");
                GetItemString(item).ForEach(o => sr.WriteLine(o));
                sr.WriteLine("");
            }

            sr.Close();
            sr2.Close();
            Process.Start("explorer", Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @$"\Hero_Siege\hseditor\export");
        }

        private static void WriteMassPages(List<string> Satanic, List<string> Set, List<string> Heroic, List<string> Angelics, StreamWriter sr, string slot)
        {
            sr.WriteLine($"<!--{slot}-->");
            if (Satanic.Count > 0)
            {
                sr.WriteLine("<!--Satanic}-->");
                sr.WriteLine("{|style=\"width: 100%\"  style=\"text-align:left\"");
                sr.WriteLine("!");
                sr.WriteLine(String.Join(string.Empty, Satanic));
                sr.WriteLine("|}");
            }
            if (Set.Count > 0)
            {
                sr.WriteLine("<!--Set}-->");
                sr.WriteLine("{|style=\"width: 100%\"  style=\"text-align:left\"");
                sr.WriteLine("!");
                sr.WriteLine(String.Join(string.Empty, Set));
                sr.WriteLine("|}");
            }
            if (Heroic.Count > 0)
            {
                sr.WriteLine("<!--Heroic}-->");
                sr.WriteLine("{|style=\"width: 100%\"  style=\"text-align:left\"");
                sr.WriteLine("!");
                sr.WriteLine(String.Join(string.Empty, Heroic));
                sr.WriteLine("|}");
            }
            if (Angelics.Count > 0)
            {
                sr.WriteLine("<!--Angelic}-->");
                sr.WriteLine("{|style=\"width: 100%\"  style=\"text-align:left\"");
                sr.WriteLine("!");
                sr.WriteLine(String.Join(string.Empty, Angelics));
                sr.WriteLine("|}");
            }
            sr.WriteLine("");
        }

        public static List<string> GetItemString(Item item2)
        {
            Item item = item2.DeepCopy();
            item.Quality = 100; item.UpgradeLevel = 1;
            item.Stats.Calculate(item);

            List<string> str = new List<string>();
            str.Add("{{Items");
            str.Add($"|title1=[[{item.Name}]]");
            str.Add($"|image1={item.Name.ToLower()}.png");
            str.Add($"|rarity={(item.Rarity.Name == "Set" ? "Satanic Set" : item.Rarity.Name)}");
            if (item.Stats.Class != null)
                str.Add($"|class=[[{item.Stats.Class.Name}]]");
            if (item.Rarity.Name == "Set")
                str.Add($"|set=[[{item.Set.Name}]]");
            str.Add($"|tier={item.Stats.Tier.Name}");
            str.Add($"|type={(item.Slot.ID == 3 ? item.WeaponType.Name : item.Slot.Name)}");
            if (item.Slot.ID == 3)
                str.Add($"|handed={item.Stats.HandedType}h");

            if (item.Stats.DamageTypes.Count != 0)
            {
                string s = "";
                string s2 = "";
                item.Stats.DamageTypes.ForEach(o => { s += $"[[file:{o.Name} Damage.png|15px]] "; s2 += $"{o.Name} Damage "; });
                s = s.Remove(s.Length - 1);
                s2 = s2.Remove(s2.Length - 1);
                str.Add($"|element={s}");
                str.Add($"|damage_type={s2}");
            }
            str.Add($"|sockets={item.Stats.SocketString.Replace(" Sockets", String.Empty)}");
            str.Add($"|upgrade_price={item.Stats.UpgradePrice}");
            str.Add($"|vendor_price={item.Stats.VendorPrice}");
            str.Add($"|lvl_req={item.Stats.LevelRequirement}");
            if (item.Rarity.Name == "Heroic")
                str.Add($"|drop_locations=[[Heroic|Drop Tables]]");
            if (item.Slot.ID == 3)
            {
                str.Add($"|base_damage={item.Stats.Damage}");
                str.Add($"|attack_speed={item.Stats.APS.ToString().Replace(',', '.').Replace(" APS", String.Empty)}");
            }
            List<Stat> randomTalents = new List<Stat>();
            foreach (Stat stat in item.Stats.StatList)
            {
                if (stat.WíkiName == "randomtalent1_18") { randomTalents.Add(stat); continue; }
                str.Add($"|{stat.WíkiName}={stat.ValueFormatted}");
            }
            if (randomTalents.Count > 0)
            {
                string s = "";
                randomTalents.ForEach(o => { s += $"[[{o.Name}|{o.Name} +{o.Value}]]"; if (randomTalents.IndexOf(o) != randomTalents.Count - 1) s += Environment.NewLine; });
                str.Add($"|talent={s}");
            }
            string ability = "";
            if (item.Stats.Ability != null && item.Stats.Ability != "")
                ability += $"[[{item.Stats.Ability}|{item.Stats.AbilityString}]]";
            if (item.Stats.Aura != null && item.Stats.Aura != "")
            {
                ability = ability == "" ? ability : ability + Environment.NewLine;
                ability += $"[[{item.Stats.Aura}|{item.Stats.AuraString}]]";
            }
            if (ability != "") str.Add($"|ability={ability}");

            item.UpgradeLevel = 10;
            item.Stats.Calculate(item);

            if (item.Slot.ID == 3)
            {
                str.Add($"|base_damage10={item.Stats.Damage}");
                str.Add($"|attack_speed10={item.Stats.APS.ToString().Replace(',', '.').Replace(" APS", String.Empty)}");
            }
            foreach (Stat stat in item.Stats.StatList)
            {
                if (stat.Multiplier == "null") continue;
                str.Add($"|{stat.WíkiName}10={stat.ValueFormatted}");
            }
            str.Add("}}");
            return str;
        }
    }
}
