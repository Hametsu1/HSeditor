using System.Collections.Generic;

namespace HSeditor.Classes.Items
{
    public class SaveItem
    {
        public int type { get; set; }
        public int id { get; set; }
        public string account { get; set; }
        public int market_id { get; set; }
        public int token_level { get; set; }
        public int weapon_type { get; set; }
        public int drop_quality { get; set; }
        public string timestamp { get; set; }
        public int socket_1 { get; set; }
        public int socket_2 { get; set; }
        public int socket_3 { get; set; }
        public int socket_4 { get; set; }
        public int socket_5 { get; set; }
        public int socket_6 { get; set; }
        public int seed { get; set; }
        public int mf_drop { get; set; }
        public int amount { get; set; }
        public int rarity { get; set; }
        public int token { get; set; }
        public double tier { get; set; }
        public double x { get; set; }
        public double y { get; set; }

        public SaveItem(int type, int id, string account, int market_id, int token_level, int weapon_type, int drop_quality, string timestamp, int seed, int mf_drop, int amount, int rarity, int token, double tier, Sockets Sockets, double x = 0, double y = 0)
        {
            this.type = type;
            this.id = id;
            this.account = account;
            this.market_id = market_id;
            this.token_level = token_level;
            this.weapon_type = weapon_type;
            this.drop_quality = drop_quality;
            this.timestamp = timestamp;
            this.seed = seed;
            this.mf_drop = mf_drop;
            this.amount = amount;
            this.rarity = rarity;
            this.token = token;
            this.tier = tier;
            this.x = x;
            this.y = y;
            List<Rune> runes = Sockets == null ? new Sockets(new List<Rune>()).GetRuneList() : Sockets.GetRuneList();
            this.socket_1 = runes[0].ID;
            this.socket_2 = runes[1].ID;
            this.socket_3 = runes[2].ID;
            this.socket_4 = runes[3].ID;
            this.socket_5 = runes[4].ID;
            this.socket_6 = runes[5].ID;
        }
    }
}
