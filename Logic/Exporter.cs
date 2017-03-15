using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace Logic
{
    public class Exporter
    {
        public Task ToFormatAsync(string sourcePath, string repoPath, string format)
        {
            switch (format.ToLower())
            {
                case "csv":
                    return ToCsvAsync(sourcePath, repoPath);
                case "db":
                    return ToSQLiteAsync(sourcePath, repoPath);
                default:
                    throw new Exception("неподдерживаемый формат");
            }
        }

        public Task ToCsvAsync(string sourcePath, string repoPath)
        {
            try
            {
                string repoDirectory = Path.GetDirectoryName(repoPath);
                if (!Directory.Exists(repoDirectory))
                {
                    Directory.CreateDirectory(repoDirectory);
                }
                if (CanOpen(sourcePath))
                {
                    return Task.Run(() => WriteInCsv(sourcePath, repoPath));
                }
                else
                {
                    throw new Exception("The file is busy with another process");
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Task ToSQLiteAsync(string sourcePath, string repoPath)
        {
            try
            {
                string directory = Path.GetDirectoryName(repoPath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                if (CanOpen(sourcePath))
                {
                    return Task.Run(() => WriteInSQLite(sourcePath, repoPath));
                }
                else
                {
                    throw new Exception("The file is busy with another process");
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void WriteInCsv(string sourcePath, string repoPath)
        {
            using (BinaryReader reader = new BinaryReader(File.Open(sourcePath, FileMode.Open), System.Text.Encoding.ASCII))
            {
                if (reader.PeekChar() > -1)
                {
                    using (StreamWriter writer = new StreamWriter(File.Open(repoPath, FileMode.OpenOrCreate)))
                    {
                        writer.WriteLine(string.Format("{0};{1}",
                            reader.ReadInt32(), //version
                            reader.ReadString() //type
                            ));
                        writer.Flush();

                        while (reader.PeekChar() > -1)
                        {
                            writer.WriteLine(string.Format("{0};{1};{2};{3}",
                                reader.ReadInt32(), //id
                                reader.ReadInt32(), //account
                                reader.ReadDouble(), //volume
                                reader.ReadString() //comment
                                ));
                            writer.Flush();
                        }
                    }
                }
            }
        }

        public void WriteInSQLite(string sourcePath, string repoPath)
        {
            string createTableHeaderText = "CREATE TABLE Header (version INTEGER, type TEXT);";
            string createTableTradeText = "CREATE TABLE Trade (id INTEGER, account INTEGER, volume DOUBLE, comment TEXT);";
            string insertInHeaderText = "INSERT INTO 'Header' ('version', 'type') VALUES (@version, @type);";
            string insertInTradeText = "INSERT INTO 'Trade' ('id', 'account', 'volume', 'comment') VALUES (@id, @account, @volume, @comment);";

            SQLiteConnection.CreateFile(repoPath);

            using (BinaryReader reader = new BinaryReader(File.Open(sourcePath, FileMode.Open), System.Text.Encoding.ASCII))
            {
                using (SQLiteConnection connection = new SQLiteConnection(string.Format("Data Source={0};", repoPath)))
                {
                    SQLiteCommand createTableHeader = new SQLiteCommand(createTableHeaderText, connection);
                    SQLiteCommand createTableTrade = new SQLiteCommand(createTableTradeText, connection);

                    connection.Open();
                    createTableHeader.ExecuteNonQuery();
                    createTableTrade.ExecuteNonQuery();

                    createTableHeader.Dispose();
                    createTableTrade.Dispose();

                    if (reader.PeekChar() > -1)
                    {
                        SQLiteCommand insertInHeader = new SQLiteCommand(insertInHeaderText, connection);

                        insertInHeader.Parameters.Add(new SQLiteParameter("@version", reader.ReadInt32()));
                        insertInHeader.Parameters.Add(new SQLiteParameter("@type", reader.ReadString()));

                        insertInHeader.ExecuteNonQuery();

                        insertInHeader.Dispose();

                        SQLiteCommand insertInTrade = new SQLiteCommand(insertInTradeText, connection);
                        while (reader.PeekChar() > -1)
                        {
                            insertInTrade = new SQLiteCommand(insertInTradeText, connection);

                            insertInTrade.Parameters.Add(new SQLiteParameter("@id", reader.ReadInt32()));
                            insertInTrade.Parameters.Add(new SQLiteParameter("@account", reader.ReadInt32()));
                            insertInTrade.Parameters.Add(new SQLiteParameter("@volume", reader.ReadDouble()));
                            insertInTrade.Parameters.Add(new SQLiteParameter("@comment", reader.ReadString()));

                            insertInTrade.ExecuteNonQuery();

                            insertInTrade.Dispose();
                        }
                    }
                }
            }
        }

        bool CanOpen(string path)
        {
            FileStream stream = null;

            try
            {
                stream = File.Open(path, FileMode.Open);
            }
            catch (IOException)
            {
                return false;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }

            return true;
        }
    }
}
