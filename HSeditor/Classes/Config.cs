using HSeditor.Classes.Other;
using HSeditor.Classes.SaveFiles;
using HSeditor.Classes.Util;
using HSeditor.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace HSeditor
{
    public class Config
    {
        public Item NewItem { get; set; }
        public HeroInfo NewChar { get; set; }
        public bool ShowGenericRarities { get; set; }
        public Point Resolution { get; set; }

        public Config(Item NewItem, HeroInfo NewChar, bool ShowGenericRarities, Point Resolution)
        {
            this.NewItem = NewItem;
            this.NewChar = NewChar;
            this.ShowGenericRarities = ShowGenericRarities;
            this.Resolution = Resolution;
        }

    }

    public class ConfigHandler
    {
        public Config Config { get; private set; }
        public List<Item> Favorites { get; private set; }
        readonly string path = Environment.GetEnvironmentVariable("LocalAppData") + $@"\Hero_Siege\hseditor\";
        public ConfigHandler()
        {
            this.LoadConfig();
        }

        public void LoadFavorites()
        {

        }



        public void LoadConfig()
        {

        }



        public void WriteConfig(bool setToDefaults = false)
        {
        }
    }
}
