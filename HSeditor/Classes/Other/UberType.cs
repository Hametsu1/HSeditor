namespace HSeditor.Classes.Other
{
    public class UberType
    {
        public int ID { get; private set; }
        public string Name { get; private set; }
        public string Color { get; private set; }

        public UberType(int ID, string Name, string Color)
        {
            this.ID = ID;
            this.Name = Name;
            this.Color = Color;
        }
    }
}
