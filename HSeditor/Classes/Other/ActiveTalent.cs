namespace HSeditor.Classes.Other
{
    public class ActiveTalent
    {
        public int ID { get; private set; }
        public Talent Talent { get; set; }

        public ActiveTalent(int ID, Talent Talent)
        {
            this.ID = ID;
            this.Talent = Talent;
        }
    }
}
