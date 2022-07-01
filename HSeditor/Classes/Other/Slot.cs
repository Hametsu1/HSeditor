using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace HSeditor.Classes.Other
{
    public class Slot
    {
        public string Name { get; private set; }
        public int ID { get; private set; }
        public string Sprite { get; private set; }

        public Slot(string Name, int ID)
        {
            this.Name = Name;
            this.ID = ID;
            this.Sprite = @"pack://application:,,,/HSeditor;component/Resources/" + Name + ".png";
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
            this.SlotsFiltered = new List<Slot>(Slots);
            this.SlotsFiltered.Add(new Slot("All Slots", -1));
            this.SlotsFiltered = this.SlotsFiltered.OrderBy(o => o.ID).ToList();
        }

        private List<Slot> GetSlots()
        {
            List<Slot> slots = new List<Slot>();

            var result = MainWindow.INSTANCE.iniDB.Read("SELECT * FROM Slots");

            while (result.Read())
            {
                slots.Add(new Slot(result.GetString("name"), result.GetInt32("id")));
            }

            return slots;
        }

        public Slot GetSlotFromName(string name)
        {
            foreach (Slot slot in this.Slots)
                if (slot.Name == name || slot.Name + "s" == name)
                    return slot;

            return null;
        }

        public Slot GetSlotFromID(int id)
        {
            foreach (Slot slot in this.Slots)
                if (slot.ID == id)
                    return slot;

            return new Slot("Unknown Slot", id);
        }
    }
}
