using HSeditor.Classes.Item_Stats;
using HSeditor.Classes.Other;
using HSeditor.Model;
using HSeditor.SaveFiles;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace HSeditor.Classes
{
    public class Stat
    {
        public string Name { get; private set; }
        public string DebugName { get; private set; }
        public string Type { get; private set; }
        public double Value { get; set; }
        public string ValueFormatted { get; set; }
        public string Color { get; private set; }
        public string Prefix { get; private set; }
        public string Multiplier { get; private set; }
        public int Priority { get; set; }
        public bool HasPriority { get; set; }

        public Stat(string Name, string DebugName, string Type, string Multiplier, int Priority, bool HasPriority, double Value = 0)
        {
            this.Name = Name;
            this.HasPriority = HasPriority;
            this.DebugName = DebugName;
            this.Type = Type;
            this.Multiplier = Multiplier;
            this.Priority = Priority;
            this.Value = Value;
            this.ValueFormatted = Value.ToString().Replace(',', '.').Trim('-');
            this.Color = "#415bd1";
            if (this.Name == "Random Stat" || this.Name == "Random Melee Stat" || this.Name == "Random Ranged Stat" || this.Name == "Random Caster Stat" || this.Name == "Random Thorn Stat" || this.Name == "Random Tank Stat")
            {
                this.Color = "#1A6F2D";
                this.Name = this.Value > 1.0 ? this.Name + "s" : this.Name;
            }
            if (this.DebugName == "Default")
                this.Color = "#F6F6F6";
            this.Prefix = this.Value < 0.0 ? "-" : "+";
            if (Type == "%") ValueFormatted += "%";
        }

        public void ChangeValue(double value)
        {
            this.Value = value;
            this.ValueFormatted = Value.ToString().Replace(',', '.').Trim('-');
            if (Type == "%") ValueFormatted += "%";
        }
    }

    public class StatHandler
    {
        private List<Stat> Stats { get; set; }
        public List<Stat> StatList { get; private set; }
        public List<DamageType> DamageTypes { get; private set; }
        public List<Tier> Tiers { get; private set; }
        public List<Item> DefaulStats { get; private set; }

        public StatHandler()
        {
            this.DefaulStats = new List<Item>();
            this.Stats = this.GetStats();
            this.StatList = new List<Stat>();
            foreach (Stat stat in this.Stats)
            {
                bool x = false;
                foreach (Stat Stat in StatList)
                    if (stat.Name == Stat.Name) x = true;

                if (!x)
                    StatList.Add(stat);
            }
            StatList = StatList.OrderBy(o => o.Name).ToList();
            this.DamageTypes = this.GetDamageTypes();
            this.DamageTypes.Insert(0, new DamageType("All Damage", "Default", "#F6F6F6"));
            this.Tiers = this.GetTiers();
        }

        private List<Tier> GetTiers()
        {
            List<Tier> tiers = new List<Tier>();

            var result = MainWindow.INSTANCE.iniDB.Read("SELECT * FROM Tiers");

            while (result.Read())
            {
                tiers.Add(new Tier(result.GetInt32("id"), result.GetString("name")));
            }
            return tiers;
        }

        public List<Stat> GetFilteredStats()
        {
            List<Stat> stats = new List<Stat>();
            List<string> Names = new List<string>();
            foreach (Stat stat in StatList)
            {
                if (Names.Contains(stat.Name)) continue;
                Names.Add(stat.Name);
                stats.Add(stat);
            }
            return stats;
        }

        public List<Stat> GetRelicStats()
        {
            List<Stat> statList = new List<Stat>();

            foreach (Relic relic in MainWindow.INSTANCE.RelicHandler.Relics)
            {
                foreach (Stat stat in relic.Stats)
                {
                    if (!ContainsStat(statList, stat))
                        statList.Add(stat);
                }
            }
            statList = statList.OrderBy(o => o.Name).ToList();
            statList.Insert(0, new Stat("All Stats", "Default", "Default", "0.0", -1, false));
            return statList;
        }

        private bool ContainsStat(List<Stat> List, Stat Stat)
        {
            foreach (Stat Stat2 in List)
                if (Stat2.Name == Stat.Name)
                    return true;
            return false;
        }

        public DamageType GetDamageType(string name)
        {
            foreach (DamageType damageType in this.DamageTypes)
                if (damageType.DebugName == name)
                    return damageType;
            return null;
        }

        public Tier GetTier(string Tier)
        {
            foreach (Tier tier in this.Tiers)
                if (tier.Name == Tier)
                    return tier;
            return null;
        }

        private List<DamageType> GetDamageTypes()
        {
            List<DamageType> damageTypes = new List<DamageType>();

            var result = MainWindow.INSTANCE.iniDB.Read("SELECT * FROM DamageTypes");

            while (result.Read())
            {
                damageTypes.Add(new DamageType(result.GetString("name"), result.GetString("debugname"), result.GetString("color")));
            }

            return damageTypes;
        }

        private List<Stat> GetStats()
        {
            List<Stat> stats = new List<Stat>();

            var result = MainWindow.INSTANCE.iniDB.Read("SELECT * FROM Stats");

            while (result.Read())
            {
                stats.Add(new Stat(result.GetString("name"), result.GetString("debugname"), result.GetString("type"), result.GetString("scalefactor"), result.GetInt32("priority"), false));
            }
            return stats;
        }

        public Stat GetStat(string stat)
        {
            foreach (Stat Stat in Stats)
                if ("[" + Stat.DebugName + "]" == stat)
                    return Stat;
            return null;
        }

        public Stat GetStat(string stat, string type)
        {
            foreach (Stat Stat in Stats)
                if (Stat.Name == stat && Stat.Type == type)
                    return Stat;
            return null;
        }
        public void ReadStats()
        {
            #region old
            //JArray result = JArray.Parse(File.ReadAllText(Environment.CurrentDirectory + @"\stats.json"));

            //foreach (JObject slot in result.Children())
            //{
            //    int id = 0;
            //    string[] temp = slot["Name"].ToString().Split(' ');
            //    string slotname = temp[0];
            //    if (slotname == "Chests") slotname = "Armor";
            //    int slotid = MainWindow.INSTANCE.SlotHandler.GetSlotFromName(slotname) == null ? 3 : MainWindow.INSTANCE.SlotHandler.GetSlotFromName(slotname).ID;
            //    int weapontypeid = slotid == 3 ? MainWindow.INSTANCE.WeaponTypeHandler.GetWeaponTypeFromName(temp[0]).ID : 0;

            //    foreach (JObject item in slot["Items"])
            //    {
            //        Item Item = MainWindow.INSTANCE.ItemHandler.GetItem(id, slotid, weapontypeid);
            //        if (Item == null) continue;
            //        List<DamageType> DamageTypes = new List<DamageType>();

            //        if (item["Stats"]["lootInfoRandomType"] != null)
            //        {
            //            string[] x = item["Stats"]["lootInfoRandomType"].ToString().Replace("[", String.Empty).Replace("]", String.Empty).Replace("/", String.Empty).Replace("*", String.Empty).Split(", ");
            //            foreach (string y in x)
            //                DamageTypes.Add(this.GetDamageType(y));
            //        }
            //        if (DamageTypes.Count == 0)
            //        {
            //            if (item["Stats"]["lootInfoDamageType"] != null && item["Stats"]["lootInfoDamageType"].ToString() != "eDtype.nothing")
            //                DamageTypes.Add(this.GetDamageType(item["Stats"]["lootInfoDamageType"].ToString()));
            //        }

            //        string? ability = item["Stats"]["[INV_ABILITY]"] == null ? null : item["Stats"]["[INV_ABILITY]"].ToString();
            //        if (ability != null)
            //        {
            //            string tempstr = ability.Remove(0, 9);
            //            ability = "";
            //            for (int i = 0; i < tempstr.Length; i++)
            //            {
            //                if (Char.IsUpper(tempstr[i]))
            //                {
            //                    ability += " " + tempstr[i];
            //                    continue;
            //                }
            //                ability += i == 0 ? Char.ToUpper(tempstr[i]) : tempstr[i];
            //            }
            //        }

            //        Stats stats = new Stats
            //        {
            //            AbilityLevelMin = Convert.ToInt32(item["Stats"]["lootAbilityLevelMin"]),
            //            AbilityLevelMax = Convert.ToInt32(item["Stats"]["lootAbilityLevelMax"]),
            //            SocketsMin = Convert.ToInt32(item["Stats"]["lootSocketsMin"]),
            //            SocketsMax = Convert.ToInt32(item["Stats"]["lootSocketsMax"]),
            //            LevelRequirement = Convert.ToInt32(item["Stats"]["[INV_LEVEL_REQ]"]),
            //            Tier = this.GetTier(item["Stats"]["lootInfoTier"].ToString().Remove(0, 6)),
            //            UpgradePrice = Convert.ToInt32(item["Stats"]["lootInfoUpgradePrice"]),
            //            Ability = ability,
            //            HandedType = Convert.ToInt32(item["Stats"]["lootInfoWeaponHanded"]) == 0 ? Item.WeaponType.DefaultHandedType : Convert.ToInt32(item["Stats"]["lootInfoWeaponHanded"]),
            //            Damage = item["Stats"]["[INV_DAMAGE]"] != null ? Convert.ToInt32(item["Stats"]["[INV_DAMAGE]"]) : null,
            //            APS = item["Stats"]["[INV_ATTACK_SPEED]"] != null ? Convert.ToDouble(item["Stats"]["[INV_ATTACK_SPEED]"]) : null
            //        };
            //        List<Stat> StatList = new List<Stat>();
            //        id++;
            //        bool hasdamagestat = false;
            //        foreach (JProperty stat in item["Stats"])
            //        {
            //            if (!stat.Name.ToString().Contains("[INV") || stat.Name.ToString().Contains("10")) continue;
            //            {
            //                Class? Class = null;
            //                Stat Stat;
            //                if (stat.Name.Contains("[INV_TALENT_"))
            //                {
            //                    Class = item["Stats"]["[INV_CLASS]"] != null ? MainWindow.INSTANCE.ClassHandler.GetClassFromName(item["Stats"]["[INV_CLASS]"].ToString().Remove(0, 7)) : null;
            //                    if (Class != null)
            //                        Stat = new Stat(Class.Talents.GetTalentFromID(Convert.ToInt32(stat.Name.Trim('[').Trim(']').Remove(0, 11))).Name + $" [{Class.Name}]", stat.Name.Trim('[').Trim(']'), "flat", "0.15", Convert.ToDouble(stat.Value));
            //                    else
            //                        Stat = new Stat(stat.Name.Trim('[').Trim(']').Replace("_", " ").Remove(0, 3), stat.Name.Trim('[').Trim(']'), "flat", "0.15", Convert.ToDouble(stat.Value));
            //                    StatList.Add(Stat);
            //                    continue;
            //                }

            //                Stat = this.GetStat(stat.Name);
            //                //if (Item.Name == "Hallgar's Dreadful Wall")
            //                //    Console.WriteLine();
            //                //if (Stat == null && stat.Name != "[INV_LEVEL_REQ]" && stat.Name != "[INV_ABILITY]" && stat.Name != "[INV_ABILITY_LEVEL]" && stat.Name != "[INV_LEVEL_REQ]" && stat.Name != "[INV_SET]" && stat.Name != "[INV_CLASS]" && stat.Name != "[INV_RARITY]" && stat.Name != "[INV_AURA]" && stat.Name != "[INV_CHASE]" && stat.Name != "[INV_LORE]" && stat.Name != "[INV_EFFECT_DEF]")
            //                //    Console.WriteLine();
            //                if (Stat == null)
            //                    continue;

            //                if (Stat.Name == "Damage")
            //                    hasdamagestat = true;

            //                StatList.Add(new Stat(Stat.Name, Stat.DebugName, Stat.Type, Stat.Multiplier, Convert.ToDouble(stat.Value)));
            //            }
            //        }
            //        foreach (DamageType type in DamageTypes)
            //            if (type == null)
            //                Console.WriteLine();
            //        if (hasdamagestat)
            //            stats.DamageTypes = DamageTypes;
            //        else
            //            stats.DamageTypes = new List<DamageType>();
            //        stats.StatList = StatList;
            //        Item.Stats = stats;
            //        Item.Stats.Set();
            //        this.DefaulStats.Add(Item.DeepCopy());
            //        Item.Stats.Calculate(Item);
            //        if (Item.Rarity.Name == "Set")
            //        {
            //            foreach (Item item2 in MainWindow.INSTANCE.ItemHandler.AllItems)
            //                if (item2.Name == Item.Name && item2.Rarity.Name == "Heroic Set")
            //                {
            //                    item2.Stats = Item.Stats.DeepCopy();
            //                    item2.Stats.Set();
            //                    item2.Stats.Calculate(item2);
            //                    break;
            //                }
            //        }
            //    }
            //}
            #endregion old
            try
            {
                var parser = new FileIniDataParser();
                IniData data = parser.ReadData(new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(Assembly.GetExecutingAssembly().GetManifestResourceNames().Single(str => str.EndsWith("stats.ini")))));
                foreach (SectionData section in data.Sections)
                {
                    Item item = MainWindow.INSTANCE.ItemHandler.GetItem(section.SectionName);
                    if (item == null) continue;
                    Stats stats = new Stats
                    {
                        AbilityLevelMin = section.Keys["lootAbilityLevelMin"] == null ? section.Keys["itemData[INV_ABILITY_LEVEL]"] == null ? 0 : Convert.ToInt32(section.Keys["itemData[INV_ABILITY_LEVEL]"]) : Convert.ToInt32(section.Keys["lootAbilityLevelMin"]),
                        AbilityLevelMax = section.Keys["lootAbilityLevelMin"] == null ? section.Keys["itemData[INV_ABILITY_LEVEL]"] == null ? 0 : Convert.ToInt32(section.Keys["itemData[INV_ABILITY_LEVEL]"]) : Convert.ToInt32(section.Keys["lootAbilityLevelMax"]),
                        AuraLevelMin = section.Keys["lootAuraLevelMin"] == null ? 0 : Convert.ToInt32(section.Keys["lootAuraLevelMin"]),
                        AuraLevelMax = section.Keys["lootAuraLevelMax"] == null ? 0 : Convert.ToInt32(section.Keys["lootAuraLevelMax"]),
                        LevelRequirement = Convert.ToInt32(section.Keys["itemData[INV_LEVEL_REQ]"]),
                        UpgradePrice = Convert.ToInt32(section.Keys["lootInfoUpgradePrice"]),
                        Class = section.Keys["itemData[INV_CLASS]"] == null ? null : MainWindow.INSTANCE.ClassHandler.GetClassFromName(section.Keys["itemData[INV_CLASS]"].Replace("eClass.", String.Empty).Replace("Locked", String.Empty)),

                        Ability = MainWindow.INSTANCE.AbilityHandler.GetAbilityFromName(section.Keys["itemData[INV_ABILITY]"]) == null
                            ? Util.Util.MakeUpper(section.Keys["itemData[INV_ABILITY]"]) == "" ? null : Util.Util.MakeUpper(section.Keys["itemData[INV_ABILITY]"])
                            : MainWindow.INSTANCE.AbilityHandler.GetAbilityFromName(section.Keys["itemData[INV_ABILITY]"]).Name,

                        Aura = Util.Util.MakeUpper(section.Keys["itemData[INV_AURA]"], 6) == "" ? null : Util.Util.MakeUpper(section.Keys["itemData[INV_AURA]"], 6),
                    };
                    stats.Tier = this.GetTier("S");
                    stats.Ability = stats.Ability == null ? Util.Util.MakeUpper(section.Keys["itemData[INV_EFFECT_DEF]"], 9) == "" ? null : Util.Util.MakeUpper(section.Keys["itemData[INV_EFFECT_DEF]"], 9) : stats.Ability;
                    if (item.Rarity.Name != "Runeword")
                    {
                        stats.SocketsMin = Convert.ToInt32(section.Keys["lootSocketsMin"]);
                        stats.SocketsMax = Convert.ToInt32(section.Keys["lootSocketsMax"]);
                        stats.Tier = this.GetTier(section.Keys["lootInfoTier"].ToString().Remove(0, 6));
                    }

                    if (item.Slot.ID == 3)
                    {
                        string aps = section.Keys["itemData[INV_ATTACK_SPEED]"];
                        stats.HandedType = Convert.ToInt32(section.Keys["lootInfoWeaponHanded"]);
                        stats.Damage = Convert.ToInt32(section.Keys["itemData[INV_DAMAGE]"]);
                        stats.APS = aps.Length == 1 ? aps : aps.Split('.')[1].Length == 1 ? aps + "0 APS" : aps + " APS";
                    }

                    List<DamageType> DamageTypes = new List<DamageType>();
                    if (section.Keys["lootInfoDamageType"] == null && section.Keys["lootInfoRandomType"] != null && section.Keys["lootInfoRandomType"] != "[eDtype.nothing]")
                        foreach (string s in section.Keys["lootInfoRandomType"].Split(','))
                        {
                            DamageType damageType = this.GetDamageType(s.Replace("[", String.Empty).Replace("]", String.Empty));
                            if (damageType == null) continue;
                            DamageTypes.Add(damageType);
                        }
                    { }
                    stats.DamageTypes = DamageTypes;


                    List<Stat> Stats = new List<Stat>();
                    foreach (KeyData key in section.Keys)
                    {
                        Stat Stat;
                        if (key.KeyName.StartsWith("itemData[INV_TALENT_"))
                        {
                            if (stats.Class == null) continue;
                            Stat = new Stat(stats.Class.Talents.GetTalentFromID(Convert.ToInt32(key.KeyName.Trim('[').Trim(']').Remove(0, 20))).Name, key.KeyName.Replace("[", String.Empty).Replace("]", String.Empty).Replace("itemData", string.Empty), "flat", "0", 67, false, Double.Parse(key.Value, CultureInfo.InvariantCulture));
                            Stats.Add(Stat);
                            continue;
                        }
                        Stat stat = this.GetStat(key.KeyName.Replace("itemData", String.Empty));
                        if (stat == null) continue;

                        Stats.Add(new Stat(stat.Name, stat.DebugName, stat.Type, stat.Multiplier, stat.Priority, stat.HasPriority, Double.Parse(key.Value, CultureInfo.InvariantCulture)));
                    }
                    stats.StatList = Stats;

                    item.Stats = stats;
                    item.Stats.Set(item);
                    this.DefaulStats.Add(item.DeepCopy());
                    item.Stats.Calculate(item);
                    if (item.Rarity.Name == "Set")
                    {
                        foreach (Item item2 in MainWindow.INSTANCE.ItemHandler.AllItems)
                            if (item2.Name == item.Name && item2.Rarity.Name == "Heroic Set")
                            {
                                item2.Stats = item.Stats.DeepCopy();
                                item2.Stats.Set(item2);
                                item2.Stats.Calculate(item2);
                                break;
                            }
                    }
                }
            }
            catch { }


        }
    }
}
