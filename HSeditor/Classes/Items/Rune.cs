using HSeditor.Classes.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace HSeditor
{

    public class Rune
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Sprite { get; set; }



        public Rune(int iD, string name, string sprite = null)
        {
            ID = iD;
            Name = name;
            Sprite = sprite;
            if (Sprite == null) this.Sprite = @"pack://application:,,,/HSeditor;component/Resources/Socketable.png";
        }
    }

    public class RuneHandler
    {
        private readonly string _default = Environment.CurrentDirectory + @$"\Sprites\Items\Socketable\";
        public List<Rune> Runes { get; private set; }
        public List<string> RuneSprites { get; set; }

        public RuneHandler()
        {
            this.RuneSprites = Directory.GetFiles(_default).ToList();
            this.Runes = GetRunes();
            this.Runes.Insert(0, new Rune(0, "None"));
        }

        public Rune GetRuneFromName(string name)
        {
            return this.Runes.Find(o => o.Name == name);
        }

        private List<Rune> GetRunes()
        {
            List<Rune> runes = new List<Rune>();

            var result = MainWindow.INSTANCE.iniDB.Read("SELECT Items.ingameid, Items.name FROM Items LEFT JOIN Slots ON Items.slotid = Slots.editorid WHERE Slots.id = 15");

            while (result.Read())
            {
                Rune item = new Rune(
                    result.GetInt32("ingameid"),
                    result.GetString("name"),
                   this.RuneSprites.Find(o => Path.GetFileNameWithoutExtension(o).Clean().ToLower() == result.GetString("name").Clean().ToLower()));

                runes.Add(item);
            }
            result.Close();

            return runes;
        }

        public Rune GetRuneFromID(int id)
        {
            Rune rune = this.Runes.Find(o => o.ID == id);
            return rune == null ? new Rune(id, "Unknown Rune", Util.GetSprite("Socketable.png")) : rune;
        }
    }
}
