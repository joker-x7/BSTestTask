using System;
using System.Collections.Generic;
using System.IO;
using Entities;

namespace Logic
{
    public class BinaryHelper
    {
        public void Write(string path, CustomFile file)
        {
            try
            {
                using (BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.OpenOrCreate)))
                {
                    writer.Write(file.Header.version);
                    writer.Write(file.Header.type);

                    foreach (TradeRecord trade in file.Trades)
                    {
                        writer.Write(trade.id);
                        writer.Write(trade.account);
                        writer.Write(trade.volume);
                        writer.Write(trade.comment);
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public CustomFile Reade(string path)
        {
            CustomFile file = new CustomFile();

            try
            {
                using (BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open), System.Text.Encoding.ASCII))
                {
                    if (reader.PeekChar() > -1)
                    {
                        Header header = new Header();
                        header.version = reader.ReadInt32();
                        header.type = reader.ReadString();

                        file.Header = header;

                        while (reader.PeekChar() > -1)
                        {
                            TradeRecord trade = new TradeRecord();

                            trade.id = reader.ReadInt32();
                            trade.account = reader.ReadInt32();
                            trade.volume = reader.ReadDouble();
                            trade.comment = reader.ReadString();

                            file.Trades.Add(trade);
                        }
                    }
                }

            }
            catch (Exception e)
            {
                throw e;
            }
            return file;
        }
    }
}
