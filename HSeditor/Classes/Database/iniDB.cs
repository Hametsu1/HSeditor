using Microsoft.Data.Sqlite;

namespace HSeditor.Classes.iniDB
{
    public class iniDB
    {
        public SqliteConnection Connection { get; private set; }

        public iniDB(string path)
        {
            this.Connection = new SqliteConnection($"Data Source={path}");
            this.Connection.Open();
        }

        public void Write(string query)
        {
            SqliteCommand command = new SqliteCommand(query, this.Connection);
            command.ExecuteNonQuery();
        }

        public SqliteDataReader Read(string query)
        {
            SqliteCommand command = new SqliteCommand(query, this.Connection);
            return command.ExecuteReader();
        }
    }
}
