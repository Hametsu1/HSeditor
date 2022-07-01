namespace HSeditor.Classes.Item_Stats
{
    public class DamageType
    {
        public string Name { get; private set; }
        public string DebugName { get; private set; }
        public string Sprite { get; private set; }
        public string Color { get; private set; }

        public DamageType(string Name, string DebugName, string Color)
        {
            this.Name = Name;
            this.DebugName = DebugName;
            this.Sprite = @"pack://application:,,,/HSeditor;component/Resources/" + Name + ".png";
            this.Color = Color;
        }
    }
}
