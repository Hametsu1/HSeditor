using HSeditor.Classes.Filter.Item;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSeditor.Classes.Items
{
    public class Stat
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string FilteredDescription { get { return this.isPercentage && MainWindow.INSTANCE.StatHandler.Stats.FindAll(o => o.Description == this.Description).Count > 1 ? this.Description + " (%)" : this.Description; } }
        public string ValueMin { get; set; }
        public string ValueMax { get; set; }
        public bool isRange { get; set; }
        public bool isMinNegative { get { return this.ValueMin.StartsWith("-"); } set { } }
        public bool isMaxNegative { get { return this.ValueMax.StartsWith("-"); } set { } }
        public bool isPercentage { get; set; }
        public bool isFiltered
        {
            get
            {
                StatFilter stat = MainWindow.INSTANCE.ItemHandler.Filter.StatFilter.Find(o => o.Stat.Name == this.Name);
                return stat == null || !stat.Selected ? false : true;
            }
        }

        public Stat(string name, string description, bool isPercentage, string value = "")
        {
            this.Name = name;
            this.Description = description;
            this.isPercentage = isPercentage;
            this.isRange = value.Trim('-').Contains("-");
            this.ValueMin = value;
            this.ValueMax = "";

            if (isRange)
            {
                bool isNegative = this.isMinNegative;
                string val = this.ValueMin;
                if (isNegative) val = val.Trim('-');
                string[] values = val.Split(new[] { '-' }, 2);
                this.ValueMin = values[0];
                if (isNegative) this.ValueMin = "-" + this.ValueMin;
                this.ValueMax = values[1];
            }
            if (!this.ValueMin.StartsWith("+") && !this.ValueMin.StartsWith("-")) this.ValueMin = isMinNegative ? "-" + this.ValueMin : "+" + this.ValueMin;
            if (!isRange && isPercentage) this.ValueMin += "%";
            else if (isRange && isPercentage) this.ValueMax += "%";
        }
    }

    public class StatHandler
    {
        public List<Stat> Stats { get; set; }

        public StatHandler()
        {
            this.Stats = this.GetStats();
        }

        public List<Stat> GetStats()
        {
            List<Stat> stats = new List<Stat>();

            var result = MainWindow.INSTANCE.iniDB.Read("SELECT * FROM Stats");

            while (result.Read())
            {
                stats.Add(new Stat(result.GetString("name"), result.GetString("description"), result.GetString("type") == "%"));
            }
            result.Close();

            return stats;
        }

        public Stat GetStatByName(string name)
        {
            Stat stat = this.Stats.Find(o => o.Name == name);
            return stat == null ? stat : new Stat(stat.Name, stat.Description, stat.isPercentage);
        }

        public List<Stat> GetStatList(string stats)
        {
            List<Stat> returnlist = new List<Stat>();

            List<string> statList = stats.Split("|").ToList();
            statList.ForEach(stat =>
            {
                try
                {
                    string[] parts = stat.Split(new[] { ' ' }, 2);
                    string value = parts[0];

                    Stat st = this.GetStatByName(parts[1]);
                    string description = st == null ? parts[1] : st.Description;
                    bool percentage = st == null ? false : st.isPercentage;
                    returnlist.Add(new Stat(parts[1], description, percentage, value));
                }
                catch { }
            });
            return returnlist;
        }
    }
}
