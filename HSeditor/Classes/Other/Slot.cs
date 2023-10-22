using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace HSeditor.Classes.Other
{
    public class Slot
    {
        public string Name { get; private set; }
        public string UniqueName { get; private set; }
        public int ID { get; private set; }
        public int EditorID { get; private set; }
        public string Sprite { get; private set; }
        public bool ShowAmount { get; private set; }
        public bool ShowRunes { get; private set; }
        public bool ShowAugment { get; private set; }

        public Slot(string Name, int ID, int? editorID = null, string uniqueName = null, bool showAmount = false, bool showRunes = true, bool showAugment = false)
        {
            if (editorID == null) editorID = ID;
            if (uniqueName == null) uniqueName = Name;
            this.Name = Name;
            this.UniqueName = uniqueName;
            this.ID = ID;
            this.EditorID = (int)editorID;
            this.Sprite = @"pack://application:,,,/HSeditor;component/Resources/" + Name + ".png";
            this.ShowAmount = showAmount;
            this.ShowRunes = showRunes;
            this.ShowAugment = showAugment;
        }
    }

    public class SlotHandler
    {
        public List<Slot> Slots { get; private set; }
        public List<Slot> SlotsFiltered { get; private set; }
        public Slot SelectedItemSlot { get; set; }

        public SlotHandler()
        {
            this.Slots = this.GetSlots();
            this.SlotsFiltered = new List<Slot>();
            this.Slots.ForEach(o =>
            {
                if (SlotsFiltered.Find(o2 => o2.Name == o.Name) == null)
                    SlotsFiltered.Add(o);
            });
            this.SlotsFiltered.Add(new Slot("All Slots", -1));
            this.SlotsFiltered = this.SlotsFiltered.OrderBy(o => o.ID).ToList();
        }

        private List<Slot> GetSlots()
        {
            List<Slot> slots = new List<Slot>();

            var result = MainWindow.INSTANCE.iniDB.Read("SELECT * FROM Slots");

            while (result.Read())
            {
                slots.Add(new Slot(result.GetString("name"), result.GetInt32("id"), result.GetInt32("editorid"), result.GetString("uniqueName"), result.GetBoolean("showamount"), result.GetBoolean("showrunes"), result.GetBoolean("showaugment")));
            }

            return slots;
        }

        public Slot GetSlotFromName(string name)
        {
            return this.Slots.Find(o => o.Name == name || o.Name + "s" == name);
        }

        public Slot GetSlotFromUniqueName(string name)
        {
            return GetSlotFromName(name);
        }

        public Slot GetSlotFromID(int id)
        {
            foreach (Slot slot in this.Slots)
                if (slot.ID == id)
                    return slot;

            return new Slot("Unknown Slot", id);
        }

        public Slot GetSlotFromEditorID(int id)
        {
            if (id >= 30)
                System.Console.WriteLine();
            return this.Slots.Find(o => o.EditorID == id);
        }
    }
}
