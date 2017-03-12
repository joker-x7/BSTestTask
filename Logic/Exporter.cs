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
        public void ToFormat(CustomFile file, string path, string format)
        {
            switch (format.ToLower())
            {
                case "csv":
                    ToCsv(file, path);
                    break;
                case "db":
                    ToSQLite(file, path);
                    break;
                default:
                    throw new Exception("неподдерживаемый формат");
            }
        }

        public void ToCsv(CustomFile file, string path)
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                sb.Append(nameof(Header.version)).Append(";");
                sb.Append(nameof(Header.type));
                sb.AppendLine();

                sb.Append(file.Header.version).Append(";");
                sb.Append(file.Header.type);
                sb.AppendLine();

                sb.Append(nameof(TradeRecord.id)).Append(";");
                sb.Append(nameof(TradeRecord.account)).Append(";");
                sb.Append(nameof(TradeRecord.volume)).Append(";");
                sb.Append(nameof(TradeRecord.comment));
                sb.AppendLine();

                foreach (TradeRecord trade in file.Trades)
                {
                    sb.Append(trade.id).Append(";");
                    sb.Append(trade.account).Append(";");
                    sb.Append(trade.volume).Append(";");
                    sb.Append(trade.comment);
                    sb.AppendLine();
                }

                File.WriteAllText(path, sb.ToString());
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void ToSQLite(CustomFile file, string path)
        {
            NumberFormatInfo volumeFormatInfo = new NumberFormatInfo();
            volumeFormatInfo.NumberDecimalSeparator = ".";

            string createTableHeaderText = "CREATE TABLE Header (version INTEGER, type TEXT);";
            string createTableTradeText = "CREATE TABLE Trade (id INTEGER PRIMARY KEY, account INTEGER, volume DOUBLE, comment TEXT);";
            string insertInHeaderText = string.Format("INSERT INTO 'Header' ('version', 'type') VALUES ({0}, '{1}');", file.Header.version, file.Header.type);
            string tradesValues = string.Join(",", file.Trades.Select(x => string.Format("({0}, {1}, {2}, '{3}')", x.id, x.account, x.volume.ToString(volumeFormatInfo), x.comment)));
            string insertInTradeText = string.Format("INSERT INTO 'Trade' ('id', 'account', 'volume', 'comment') VALUES {0};", tradesValues);

            SQLiteConnection.CreateFile(path);

            using (SQLiteConnection connection = new SQLiteConnection(string.Format("Data Source={0};", path)))
            {
                try
                {
                    SQLiteCommand createTableHeader = new SQLiteCommand(createTableHeaderText, connection);
                    SQLiteCommand createTableTrade = new SQLiteCommand(createTableTradeText, connection);
                    SQLiteCommand insertInHeader = new SQLiteCommand(insertInHeaderText, connection);
                    SQLiteCommand insertInTrade = new SQLiteCommand(insertInTradeText, connection);

                    connection.Open();
                    createTableHeader.ExecuteNonQuery();
                    createTableTrade.ExecuteNonQuery();
                    insertInHeader.ExecuteNonQuery();
                    insertInTrade.ExecuteNonQuery();

                    createTableHeader.Dispose();
                    createTableTrade.Dispose();
                    insertInHeader.Dispose();
                    insertInTrade.Dispose();
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
    }
}
