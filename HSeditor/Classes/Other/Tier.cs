namespace HSeditor.Classes.Other
{
    public class Tier
    {
        public int ID { get; private set; }
        public string Name { get; private set; }
        public string Color { get; private set; }

        public Tier(int ID, string Name)
        {
            this.ID = ID;
            this.Name = Name;
            this.Color = this.Name == "All Tiers" ? "#F6F6F6" : "#B89250";
        }
    }
}
