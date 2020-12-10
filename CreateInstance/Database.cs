using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLite;
using System.IO;

namespace CreateInstance
{
    public class Database
    {
        SQLiteConnection dbConnection;
        string dbFile;
        public Database(string file)
        {
            dbFile = file;
            if (!File.Exists(file))
            {
                SQLiteConnection.CreateFile(file);
                Connection();
                SQLiteCommand command = new SQLiteCommand("create table user (ip varchar(512), os varchar(128), port varchar(16), time varchar(128))", dbConnection);
                command.ExecuteNonQuery();
                Close();
            }
        }

        public void Connection()
        {
            dbConnection = new SQLiteConnection($"Data Source={dbFile};Version=3;");
            dbConnection.Open();
        }

        public bool Close()
        {
            if (dbConnection != null)
            {
                dbConnection.Close();
                return true;
            }else
            {
                return false;
            }
        }
 
        public bool Add(string ip, string os, string port)
        {
            Connection();

            string sql = $"insert into user(ip, os, port, time) values('{ip}', '{os}', '{port}', '{DateTime.Now.ToString()}')";
            SQLiteCommand command = new SQLiteCommand(sql, dbConnection);
            command.ExecuteNonQuery();
            Close();

            return true;
        }

        public (string ip, string os, string port, string time)[] GetData()
        {
            var lists = new List<(string ip, string os, string port, string time)>();

            Connection();

            string sql = "select * from user";

            SQLiteCommand command = new SQLiteCommand(sql, dbConnection);
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                lists.Add(((string)reader["ip"], (string)reader["os"], (string)reader["port"], (string)reader["time"]));
            }
            Close();

            return lists.ToArray();
        }

    }
}
