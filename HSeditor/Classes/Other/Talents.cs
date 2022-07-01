using System.Collections.Generic;
using System.Linq;

namespace HSeditor.Classes.Other
{
    public class Talents
    {
        public List<Talent> TalentList { get; private set; }
        public string Name_Tree1 { get; private set; }
        public string Name_Tree2 { get; private set; }

        public Talents(Class Class, List<Talent> Talents = null)
        {
            TalentList = Talents == null ? MainWindow.INSTANCE.TalentHandler.GetClassTalents(Class.Name).Count == 0 ? MainWindow.INSTANCE.TalentHandler.GetClassTalents("Nomad") : MainWindow.INSTANCE.TalentHandler.GetClassTalents(Class.Name) : Talents;
            Name_Tree1 = GetTree1()[0].Tree;
            Name_Tree2 = GetTree2()[0].Tree;
        }

        public List<Talent> GetTree1(bool onlySet = false)
        {
            List<Talent> List_Tree1 = new List<Talent>();
            foreach (Talent talent in TalentList.OrderBy(o => o.ID))
            {
                if (talent.ID < 10)
                {
                    if (onlySet && talent.Points == 0) continue;
                    List_Tree1.Add(talent);
                }
            }
            return List_Tree1;
        }

        public List<Talent> GetTree2(bool onlySet = false)
        {
            List<Talent> List_Tree2 = new List<Talent>();
            foreach (Talent talent in TalentList.OrderBy(o => o.ID))
            {
                if (talent.ID >= 10)
                {
                    if (onlySet && talent.Points == 0) continue;
                    List_Tree2.Add(talent);
                }
            }
            return List_Tree2;
        }

        public List<Talent> GetActiveTalents()
        {
            List<Talent> talents = new List<Talent>();
            foreach (Talent talent in this.TalentList)
                if (talent.Type == "Active") talents.Add(talent);
            return talents.OrderBy(o => o.ID).ToList();
        }

        public Talent GetTalentFromID(int id)
        {
            foreach (Talent Talent in this.TalentList)
                if (Talent.ID == id)
                    return Talent;
            return null;
        }
    }
}
