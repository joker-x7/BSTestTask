using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace Logic
{
    public class FileLogic
    {
        BinaryHelper binaryHelper;
        Exporter exporter;

        public FileLogic()
        {
            binaryHelper = new BinaryHelper();
            exporter = new Exporter();
        }

        public void ExportToFormat(string sourcePath, string repoPath, string format)
        {
            CustomFile file = binaryHelper.Reade(sourcePath);
            exporter.ToFormat(file, repoPath, format);
        }

        public FileStream GetStream(string path)
        {
            try
            {
                return new FileStream(path, FileMode.Open, FileAccess.Read);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void Delete(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                else
                {
                    throw new FileNotFoundException();
                }
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public TradeRecord GetById(string path, int id)
        {
            TradeRecord trade = new TradeRecord();
            string commandText = string.Format("SELECT id, account, volume, comment FROM 'Trade' WHERE id = {0};", id);

            try
            {
                if (File.Exists(path))
                {
                    using (SQLiteConnection connection = new SQLiteConnection(string.Format("Data Source={0};", path)))
                    {
                        using (SQLiteCommand command = new SQLiteCommand(commandText, connection))
                        {
                            connection.Open();
                            using (SQLiteDataReader reader = command.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    foreach (DbDataRecord record in reader)
                                    {
                                        trade.id = (int)(long)record["id"];
                                        trade.account = (int)(long)record["account"];
                                        trade.volume = (double)record["volume"];
                                        trade.comment = record["comment"].ToString();
                                    }
                                }
                                else
                                {
                                    throw new Exception("trade record not found");
                                }
                            }
                        }
                    }
                }
                else
                {
                    throw new FileNotFoundException();
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return trade;
        }
    }
}
