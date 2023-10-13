using Newtonsoft.Json.Linq;

namespace HSeditor.Classes.SaveFiles
{
    public class SaveInventory
    {
        public JObject[] inventory_tab_0 { get; set; }
        public JObject[] inventory_tab_1 { get; set; }
        public JObject[] inventory_tab_2 { get; set; }
        public JObject[] inventory_tab_3 { get; set; }

        public JObject[] inventory_socket_tab { get; set; }
        public JObject[] inventory_potion_tab { get; set; }

        public SaveInventory(JObject[] inventory_tab_0, JObject[] inventory_tab_1, JObject[] inventory_tab_2, JObject[] inventory_tab_3, JObject[] inventory_socket_tab, JObject[] inventory_potion_tab)
        {
            this.inventory_tab_0 = inventory_tab_0;
            this.inventory_tab_1 = inventory_tab_1;
            this.inventory_tab_2 = inventory_tab_2;
            this.inventory_tab_3 = inventory_tab_3;
            this.inventory_socket_tab = inventory_socket_tab;
            this.inventory_potion_tab = inventory_potion_tab;
        }
    }

    public class SaveStash
    {
        public double stash_reset { get; set; }
        public JObject[] stash_tab_1 { get; set; }
        public JObject[] stash_tab_2 { get; set; }
        public JObject[] stash_tab_3 { get; set; }
        public JObject[] stash_tab_4 { get; set; }
        public JObject[] stash_tab_5 { get; set; }
        public JObject[] stash_socket_tab { get; set; }

        public SaveStash(JObject[] stash_tab_0, JObject[] stash_tab_1, JObject[] stash_tab_2, JObject[] stash_tab_3, JObject[] stash_tab_4, JObject[] stash_socket_tab)
        {
            this.stash_reset = 0.0;
            this.stash_tab_1 = stash_tab_0;
            this.stash_tab_2 = stash_tab_1;
            this.stash_tab_3 = stash_tab_2;
            this.stash_tab_4 = stash_tab_3;
            this.stash_tab_5 = stash_tab_4;
            this.stash_socket_tab = stash_socket_tab;
        }
    }
}
