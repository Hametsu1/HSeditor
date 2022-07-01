using System.Collections.Generic;

namespace HSeditor.Classes.Items
{
    public class Runeword
    {
        public int ID { get; private set; }
        public string Name { get; private set; }
        public int ItemLevel { get; private set; }
        public Sockets Sockets { get; private set; }
        public List<int> Bases { get; private set; }
        public int SocketCount { get; private set; }
        public Item Item { get; private set; }

        public Runeword(int ID, string Name, int ItemLevel, List<Rune> Runes, List<int> Bases, int SocketCount, Item item)
        {
            this.Sockets = new Sockets(Runes);
            this.Name = Name;
            this.ItemLevel = ItemLevel;
            this.ID = ID;
            this.Bases = Bases;
            this.SocketCount = SocketCount;
            this.Item = item;
            this.Item.ID = Bases[0];
            this.Item.Sockets = new Sockets(this.Sockets.GetRuneList());
            this.Item.SocketCount = this.SocketCount;
        }
    }
}
